using SlotSystem;
using UnityEngine;

namespace CardSystem
{
    public class CardCreator : MonoBehaviour
    {
        [SerializeField] private Card _cardPrefab;
        [SerializeField] private Transform _parentTransform;


        public Card CreateCard(Slot slot)
        {
            var card = Instantiate(_cardPrefab, _parentTransform);
            var rt = card.GetRectTransform();

            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;
            
            card.SetSlotSO(slot.GetSlotSO());
            card.SetCount(slot.GetCount());
            card.SetImage(slot.GetSlotSO().GetIcon());
            
            return card;
        }
    }
}