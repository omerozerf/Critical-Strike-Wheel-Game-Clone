using UnityEngine;

namespace ZoneSystem
{
    public class ZoneCreator : MonoBehaviour
    {
        [SerializeField] private Zone _zonePrefab;


        public void CreateZone(int zoneNumber, Vector2 rectPosition, Transform parent)
        {
            var zone = Instantiate(_zonePrefab, rectPosition, Quaternion.identity, parent);
            zone.SetZoneNumber(zoneNumber);
        }
    }
}