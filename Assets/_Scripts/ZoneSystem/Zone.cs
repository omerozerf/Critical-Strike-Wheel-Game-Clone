using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZoneSystem
{
    public class Zone : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private TMP_Text _zoneNumberText;
        [SerializeField] private Image _zoneBgImage;
        [SerializeField] private Color _normalZoneColor;
        [SerializeField] private Color _safeZoneColor;
        [SerializeField] private Color _superZoneColor;
        [SerializeField] private Color _inactiveNormalZoneColor;
        [SerializeField] private Color _inactiveSafeZoneColor;
        [SerializeField] private Color _inactiveSuperZoneColor;

        private int m_ZoneNumber;
        private ZoneType m_ZoneType;        
        
        
        private void OnValidate()
        {
            if (!_zoneNumberText)
            {
                _zoneNumberText = GetComponentInChildren<TMP_Text>();
            }
            
            if (!_zoneBgImage)
            {
                _zoneBgImage = GetComponentInChildren<Image>();
            }
            
            if (!_rectTransform)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
        }
        
        
        private void HandleZoneNumberUpdated(int zoneNumber)
        {
            bool isSuperZone = zoneNumber % 30 == 0;
            bool isSafeZone  = zoneNumber == 1 || (zoneNumber % 5 == 0);

            if (isSuperZone)
            {
                m_ZoneType = ZoneType.Super;
                _zoneNumberText.color = _superZoneColor;
                _zoneBgImage.color = _superZoneColor;
                return;
            }

            if (isSafeZone)
            {
                m_ZoneType = ZoneType.Safe;
                _zoneNumberText.color = _safeZoneColor;
                _zoneBgImage.color = _safeZoneColor;
                return;
            }

            m_ZoneType = ZoneType.Normal;
        }
        
        private void HandleZonePositionUpdated(Vector2 position)
        {
            switch (m_ZoneType)
            {
                case ZoneType.None:
                {
                    throw new InvalidOperationException("ZoneType is None.");
                }
                case ZoneType.Normal:
                {
                    _zoneNumberText.color = position.x >= 0 ? _normalZoneColor : _inactiveNormalZoneColor;
                    _zoneBgImage.color = position.x >= 0 ? _normalZoneColor : _inactiveNormalZoneColor;
                    break;
                }

                case ZoneType.Safe:
                {
                    _zoneNumberText.color = position.x >= 0 ? _safeZoneColor : _inactiveSafeZoneColor;
                    _zoneBgImage.color = position.x >= 0 ? _safeZoneColor : _inactiveSafeZoneColor;
                    break;
                }
                case ZoneType.Super:
                {
                    _zoneNumberText.color = position.x >= 0 ? _superZoneColor : _inactiveSuperZoneColor;
                    _zoneBgImage.color = position.x >= 0 ? _superZoneColor : _inactiveSuperZoneColor;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        
        public int GetZoneNumber()
        {
            return m_ZoneNumber;
        }
        
        public void SetZoneNumber(int zoneNumber)
        {
            m_ZoneNumber = zoneNumber;
            _zoneNumberText.text = m_ZoneNumber.ToString();
            
            HandleZoneNumberUpdated(zoneNumber);
        }

        public RectTransform GetRectTransform()
        {
            return _rectTransform;
        }

        public void SetPosition(Vector2 anchoredPosition, bool animate = true)
        {
            if (!animate)
            {
                _rectTransform.anchoredPosition = anchoredPosition;
            }
            else
            {
                _rectTransform.DOAnchorPos(anchoredPosition, 0.5f).SetEase(Ease.Linear);
            }

            HandleZonePositionUpdated(anchoredPosition);
        }
    }
}