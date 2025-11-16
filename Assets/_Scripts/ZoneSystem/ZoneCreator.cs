using UnityEngine;

namespace ZoneSystem
{
    public class ZoneCreator : MonoBehaviour
    {
        [SerializeField] private Zone _zonePrefab;


        public Zone CreateZone(int zoneNumber, Vector2 anchoredPos, Transform parent)
        {
            var zone = Instantiate(_zonePrefab, parent);
            var rt = zone.GetRectTransform();

            rt.anchoredPosition = anchoredPos;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;

            zone.SetZoneNumber(zoneNumber);
            
            return zone;
        }
    }
}