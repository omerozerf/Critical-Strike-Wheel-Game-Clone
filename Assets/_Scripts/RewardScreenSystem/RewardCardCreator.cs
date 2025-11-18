using CardSystem;
using UnityEngine;

namespace RewardScreenSystem
{
    public class RewardCardCreator : MonoBehaviour
    {
        [SerializeField] private RewardCard _rewardCardPrefab;
        [SerializeField] private Transform _parentTransform;


        public RewardCard CreateRewardCard(Card card)
        {
            var rewardCard = Instantiate(_rewardCardPrefab, _parentTransform);
            rewardCard.SetCardData(card);
            
            return rewardCard;
        }
    }
}