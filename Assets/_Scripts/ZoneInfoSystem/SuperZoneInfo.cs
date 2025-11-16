using Managers;
using UnityEngine;

namespace ZoneInfoSystem
{
    public class SuperZoneInfo : ZoneInfo
    {
        [SerializeField] private int _firstSuperZoneNumber;
        
        private int m_CurrentSafeZoneNumber;
        
        
        private void Start()
        {
            SetZoneNumber(_firstSuperZoneNumber);
        }


        protected override void HandleZoneChanged(int obj)
        {
            if (obj == _firstSuperZoneNumber)
            {
                SetZoneNumber(GameCommonVariableManager.GetSuperZoneInterval());
                m_CurrentSafeZoneNumber = GameCommonVariableManager.GetSuperZoneInterval();
            }
            else if (obj % GameCommonVariableManager.GetSuperZoneInterval() == 0)
            {
                SetZoneNumber(m_CurrentSafeZoneNumber + GameCommonVariableManager.GetSuperZoneInterval());
                m_CurrentSafeZoneNumber += GameCommonVariableManager.GetSuperZoneInterval();
            }
        }
    }
}