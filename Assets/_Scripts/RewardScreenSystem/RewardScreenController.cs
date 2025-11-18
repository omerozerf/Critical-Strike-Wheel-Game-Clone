using System;
using ButtonSystem;
using CardSystem;
using DG.Tweening;
using ScreenSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RewardScreenSystem
{
    public class RewardScreenController : ScreenController
    {
        [SerializeField] private CardPanelController _cardPanelController;
        [SerializeField] private RewardCardCreator _rewardCardCreator;

        private void Awake()
        {
            CollectRewardsButton.OnClicked += HandleOnClicked;
        }
        
        private void OnDestroy()
        {
            CollectRewardsButton.OnClicked -= HandleOnClicked;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            InitializeRewardCardCreator();
        }
        
        
        private void HandleOnClicked()
        {
            var list = _cardPanelController.GetCardList();
            for (var index = 0; index < list.Count; index++)
            {
                var card = list[index];
                _rewardCardCreator.CreateRewardCard(card);
            }
        }
        
        private void InitializeRewardCardCreator()
        {
            if (!_rewardCardCreator) _rewardCardCreator = GetComponent<RewardCardCreator>();
        }
        
        
        public override void Show()
        {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.DOFade(1f, _fadeInDuration).SetEase(Ease.InOutSine);
        }

        public override void Hide()
        {
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.DOFade(0f, _fadeInDuration).SetEase(Ease.InOutSine);
        }
    }
}