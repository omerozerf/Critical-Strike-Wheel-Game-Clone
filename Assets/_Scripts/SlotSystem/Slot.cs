using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlotSystem
{
    public class Slot : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _countText;

        private SlotSO m_SlotSO;

        
        public void SetSlot(SlotSO slotSO, int count)
        {
            _iconImage.sprite = slotSO.GetIcon();
            _countText.text = count.ToString();
            m_SlotSO = slotSO;
        }
        
        public SlotSO GetSlot()
        {
            return m_SlotSO;
        }
        

        private void OnValidate()
        {
            if (!_iconImage)
            {
                _iconImage = GetComponentInChildren<Image>();
            }

            if (!_countText)
            {
                _countText = GetComponentInChildren<TMP_Text>();
            }
        }
    }
}