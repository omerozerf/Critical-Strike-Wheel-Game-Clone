using System;
using UnityEngine;

namespace ZoneSystem
{
    public class ZonePanelController : MonoBehaviour
    {
        [SerializeField] private ZoneCreator _zoneCreator;
        [SerializeField] private float _zoneSpacing;
        [SerializeField] private float _zoneCurrentAroundSpacing;
        [SerializeField] private Transform _zoneParentTransform;

        private int m_CurrentZoneNumber;
        private int m_LastZoneNumber;


        private void Start()
        {
            for (int index = 0; index < 11; index++)
            {
                _zoneCreator.CreateZone(index + 1, new Vector2(index * _zoneSpacing, 0f), _zoneParentTransform);
            }

            m_CurrentZoneNumber = 1;
            m_LastZoneNumber = 10;    
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