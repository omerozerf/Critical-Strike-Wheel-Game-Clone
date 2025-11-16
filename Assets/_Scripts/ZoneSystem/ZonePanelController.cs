using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZoneSystem
{
    public class ZonePanelController : MonoBehaviour
    {
        [SerializeField] private ZoneCreator _zoneCreator;
        [SerializeField] private float _zoneSpacing;
        [SerializeField] private Transform _zoneParentTransform;

        private int m_CurrentZoneNumber;
        private int m_LastZoneNumber;
        private List<Zone> m_ZoneList = new List<Zone>();


        private void Start()
        {
            for (var index = 0; index < 11; index++)
            {
                var zone = _zoneCreator
                    .CreateZone(index + 1, new Vector2(index * _zoneSpacing, 0f), _zoneParentTransform);
                
                m_ZoneList.Add(zone);
            }

            m_CurrentZoneNumber = 0;
            m_LastZoneNumber = 10;
            
            
            NextZone();
        }

        private void NextZone()
        {
            foreach (var zone in m_ZoneList)
            {
                var anchoredPosition = zone.GetRectTransform().anchoredPosition;
                var newAnchoredPosition = new Vector2(anchoredPosition.x - _zoneSpacing, anchoredPosition.y);
                zone.SetPosition(newAnchoredPosition);
            }
        }

        private void OnValidate()
        {
            if (!_zoneCreator)
            {
                _zoneCreator = GetComponentInChildren<ZoneCreator>();
            }
        }


        public int GetCurrentZoneNumber()
        {
            return m_CurrentZoneNumber;
        }
        
        public void SetCurrentZoneNumber(int zoneNumber)
        {
            m_CurrentZoneNumber = zoneNumber;
        }
    }
}