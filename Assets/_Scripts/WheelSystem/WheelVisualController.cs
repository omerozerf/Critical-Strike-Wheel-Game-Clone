using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using ZoneSystem;

namespace WheelSystem
{
    public class WheelVisualController : MonoBehaviour
    {
        [SerializeField] private Image _wheelBgImage;
        [SerializeField] private Image _wheelIndicatorImage;
        [Space]
        [SerializeField] private Sprite _normalWheelBgSprite;
        [SerializeField] private Sprite _safeWheelBgSprite;
        [SerializeField] private Sprite _superWheelBgSprite;
        [Space]
        [SerializeField] private Sprite _normalWheelIndicatorSprite;
        [SerializeField] private Sprite _safeWheelIndicatorSprite;
        [SerializeField] private Sprite _superWheelIndicatorSprite;


        private void Awake()
        {
            ZonePanelController.OnZoneChanged += HandleZoneChanged;
        }
        
        private void OnDestroy()
        {
            ZonePanelController.OnZoneChanged -= HandleZoneChanged;
        }

        
        private void HandleZoneChanged(int zoneIndex)
        {
            var fixZone = zoneIndex + 1;
            
            if (fixZone % GameCommonVariableManager.GetSuperZoneInterval() == 0)
            {
                _wheelBgImage.sprite = _superWheelBgSprite;
                _wheelIndicatorImage.sprite = _superWheelIndicatorSprite;
            }
            else if (fixZone == 1 || fixZone % GameCommonVariableManager.GetSafeZoneInterval() == 0)
            {
                _wheelBgImage.sprite = _safeWheelBgSprite;
                _wheelIndicatorImage.sprite = _safeWheelIndicatorSprite;
            }
            else
            {
                _wheelBgImage.sprite = _normalWheelBgSprite;
                _wheelIndicatorImage.sprite = _normalWheelIndicatorSprite;
            }
        }
    }
}