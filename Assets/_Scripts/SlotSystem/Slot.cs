using System;
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
        private int m_Count;

        
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
        
        
        public void SetSlot(SlotSO slotSO, int count)
        {
            m_SlotSO = slotSO;
            m_Count = count;
            
            _iconImage.sprite = slotSO.GetIcon();
            _countText.text = count > 0
                ? $"x{count.ToString()}" 
                : string.Empty;
        }
        
        public SlotSO GetSlotSO()
        {
            return m_SlotSO;
        }
        
        public int GetCount()
        {
            return m_Count;
        }
    }
}