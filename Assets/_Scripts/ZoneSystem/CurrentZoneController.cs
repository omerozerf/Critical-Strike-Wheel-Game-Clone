using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using WheelSystem;

namespace ZoneSystem
{
    public class CurrentZoneController : MonoBehaviour
    {
        [SerializeField] private Color _normalZoneColor;
        [SerializeField] private Color _safeZoneColor;
        [SerializeField] private Color _superZoneColor;
        [SerializeField] private Image _zoneBgImage;
        [SerializeField] private Image _zoneCurrentFrameImage;


        private void Awake()
        {
            ZonePanelController.OnZoneChanged += HandleWheelStopped;
        }
        
        private void OnDestroy()
        {
            ZonePanelController.OnZoneChanged -= HandleWheelStopped;
        }
        
        
        private void HandleWheelStopped(int slotIndex)
        {
            var fixZone = slotIndex + 1;

            if (IsSuperZone(fixZone))
            {
                _zoneBgImage.color = _superZoneColor;
                _zoneCurrentFrameImage.color = _superZoneColor;
                var color = _zoneCurrentFrameImage.color;
                color.a = 1;
                _zoneCurrentFrameImage.color = color;
            }
            else if (IsSafeZone(fixZone))
            {
                _zoneBgImage.color = _safeZoneColor;
                _zoneCurrentFrameImage.color = _safeZoneColor;
                var color = _zoneCurrentFrameImage.color;
                color.a = 1;
                _zoneCurrentFrameImage.color = color;
            }
            else
            {
                _zoneBgImage.color = _normalZoneColor;
                _zoneCurrentFrameImage.color = _normalZoneColor;
                var color = _zoneCurrentFrameImage.color;
                color.a = 1;
                _zoneCurrentFrameImage.color = color;
            }
        }
        
        private bool IsSafeZone(int zone)
        {
            if (GameCommonVariableManager.GetSafeZoneInterval() <= 0) return false;
            return zone == 1 || zone % GameCommonVariableManager.GetSafeZoneInterval() == 0;
        }

        private bool IsSuperZone(int zone)
        {
            if (GameCommonVariableManager.GetSuperZoneInterval() <= 0) return false;
            return zone != 1 && zone % GameCommonVariableManager.GetSuperZoneInterval() == 0;
        }
    }
}