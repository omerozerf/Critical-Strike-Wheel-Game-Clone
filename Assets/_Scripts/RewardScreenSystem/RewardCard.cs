using CardSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RewardScreenSystem
{
    public class RewardCard : MonoBehaviour
    {
        [SerializeField] private TMP_Text _cardHeaderText;
        [SerializeField] private Image _cardImage;
        [SerializeField] private TMP_Text _cardCountText;
        
        
        public void SetCardData(Card card)
        {
            var slotSO = card.GetSlotSO();
            
            _cardHeaderText.text = slotSO.GetName().Replace(" ", "\n");
            _cardImage.sprite = slotSO.GetIcon();
            _cardCountText.text = $"x{card.GetCount()}";
        }
    }
}