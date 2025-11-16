using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZoneSystem
{
    public class ZonePanelController : MonoBehaviour
    {
        [SerializeField] private ZoneCreator _zoneCreator;
        [SerializeField] private int _zoneCount;
        [SerializeField] private float _zoneSpacing;
        [SerializeField] private Transform _zoneParentTransform;
        [SerializeField] private float _leftRecycleX;

        private readonly List<Zone> m_ZoneList = new List<Zone>();
        
        private int m_CurrentZoneNumber;
        private int m_LastZoneNumber;


        private void Start()
        {
            for (var index = 0; index < _zoneCount; index++)
            {
                var zone = _zoneCreator
                    .CreateZone(index + 1, new Vector2(index * _zoneSpacing, 0f), _zoneParentTransform);
                
                m_ZoneList.Add(zone);
            }

            m_CurrentZoneNumber = 0;
            m_LastZoneNumber = _zoneCount;
        }

        private void OnValidate()
        {
            if (!_zoneCreator)
            {
                _zoneCreator = GetComponentInChildren<ZoneCreator>();
            }
        }
        
        
        private void NextZone()
        {
            var firstZone = m_ZoneList[0];
            var rect = firstZone.GetRectTransform();
            var pos = rect.anchoredPosition;

            if (pos.x <= _leftRecycleX)
            {
                var lastZone = m_ZoneList[^1];
                var lastPos = lastZone.GetRectTransform().anchoredPosition;
                var targetX = lastPos.x + _zoneSpacing;

                firstZone.gameObject.SetActive(false);

                m_ZoneList.RemoveAt(0);

                var newPos = new Vector2(targetX, pos.y);
                firstZone.SetPosition(newPos, false);

                m_ZoneList.Add(firstZone);

                firstZone.gameObject.SetActive(true);
                m_LastZoneNumber++;
                firstZone.SetZoneNumber(m_LastZoneNumber);
            }

            foreach (var zone in m_ZoneList)
            {
                var anchoredPosition = zone.GetRectTransform().anchoredPosition;
                var newAnchoredPosition = new Vector2(anchoredPosition.x - _zoneSpacing, anchoredPosition.y);
                zone.SetPosition(newAnchoredPosition);
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