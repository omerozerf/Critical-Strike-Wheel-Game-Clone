using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SlotSystem;
using UnityEngine;
using WheelSystem;
using WheelSystem.WheelEffectSystem;

namespace CardSystem
{
    public class CardPanelController : MonoBehaviour
    {
        [SerializeField] private CardCreator _cardCreator;
        [SerializeField] private WheelSlotEffectController _wheelSlotEffectController;
        
        private readonly List<Card> m_CardList = new ();
        
        
        private void Awake()
        {
            WheelSlotSelector.OnSlotSelected += HandleSlotSelectedAsync;
        }
        
        private void OnDestroy()
        {
            WheelSlotSelector.OnSlotSelected -= HandleSlotSelectedAsync;
        }

        private void OnValidate()
        {
            InitializeCardCreator();
        }
        
        
        private async void HandleSlotSelectedAsync(Slot slot)
        {
            var moveDuration = _wheelSlotEffectController.GetMoveDuration();
            var fixedMoveDuration = moveDuration + 0.25f;
            var slotCount = slot.GetCount();
            
            foreach (var card in m_CardList)
            {
                var isSameSlotSo = card.GetSlotSO() == slot.GetSlotSO();
                if (isSameSlotSo)
                {
                    _wheelSlotEffectController.PlayAsync(slotCount, slot.GetIconRectTransform(), card.GetRectTransform(), slot.GetSlotSO().GetIcon());
                    await UniTask.WaitForSeconds(fixedMoveDuration);
                    card.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 10, 0.5f);
                    card.SetCount(card.GetCount() + slotCount);
                    return;
                }
            }
            var newCard = _cardCreator.CreateCard(slot);
            m_CardList.Add(newCard);
            await UniTask.NextFrame(); // wait for the card to be created
            _wheelSlotEffectController.PlayAsync(slot.GetCount(), slot.GetIconRectTransform(), newCard.GetRectTransform(), slot.GetSlotSO().GetIcon());
            await UniTask.WaitForSeconds(fixedMoveDuration);
            newCard.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 10, 0.5f);
            newCard.SetCount(slotCount);
        }
        
        
        private void InitializeCardCreator()
        {
            if (!_cardCreator) _cardCreator = GetComponent<CardCreator>();
        }
        
        
        public List<Card> GetCardList()
        {
            return m_CardList;
        }
    }
}