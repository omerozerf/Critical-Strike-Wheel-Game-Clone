using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class Slot : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _countText;

        
        public void SetSlot(Sprite iconSprite, int count)
        {
            _iconImage.sprite = iconSprite;
            _countText.text = count.ToString();
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