using TMPro;
using UnityEngine;
using ZoneSystem;

namespace ZoneInfoSystem
{
    public abstract class ZoneInfo : MonoBehaviour
    {
        [SerializeField] private TMP_Text _zoneNumberText;
        

        private void Awake()
        {
            ZonePanelController.OnZoneChanged += HandleZoneChanged;
        }
        
        private void OnDestroy()
        {
            ZonePanelController.OnZoneChanged += HandleZoneChanged;
        }
        
        
        protected abstract void HandleZoneChanged(int obj);
        
        
        protected TMP_Text GetZoneNumberText()
        {
            return _zoneNumberText;
        }

        protected void SetZoneNumber(int zoneNumber)
        {
            _zoneNumberText.text = zoneNumber.ToString();
        }
    }
}