using System;
using UnityEngine;
using UnityEngine.UI;

namespace WheelSystem.WheelEffectSystem
{
    public class WheelSlotEffect : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _image;


        private void OnValidate()
        {
            InitializeRectTransform();
            InitializeImage();
        }

        
        private void InitializeImage()
        {
            if (!_image) _image = GetComponentInChildren<Image>();
        }

        private void InitializeRectTransform()
        {
            if (!_rectTransform) _rectTransform = GetComponent<RectTransform>();
        }
        
        
        public RectTransform GetRectTransform()
        {
            return _rectTransform;
        }
        
        public Image GetImage()
        {
            return _image;
        }
        
        public void SetImage(Sprite sprite)
        {
            _image.sprite = sprite;
        }
    }
}