using Managers;
using UnityEngine;

namespace ZoneInfoSystem
{
    public class SafeZoneInfo : ZoneInfo
    {
        [SerializeField] private int _firstSafeZoneNumber;
        
        private int m_CurrentSafeZoneNumber;
        
        
        private void Start()
        {
            SetZoneNumber(_firstSafeZoneNumber);
            m_CurrentSafeZoneNumber = _firstSafeZoneNumber;
        }


        protected override void HandleZoneChanged(int obj)
        {
            if (obj <= 0) return;
            
            if (obj == _firstSafeZoneNumber)
            {
                SetZoneNumber(GameCommonVariableManager.GetSafeZoneInterval());
                m_CurrentSafeZoneNumber = GameCommonVariableManager.GetSafeZoneInterval();
            }
            else if (obj % GameCommonVariableManager.GetSafeZoneInterval() == 0)
            {
                SetZoneNumber(m_CurrentSafeZoneNumber + GameCommonVariableManager.GetSafeZoneInterval());
                m_CurrentSafeZoneNumber += GameCommonVariableManager.GetSafeZoneInterval();
            }
        }
    }
}