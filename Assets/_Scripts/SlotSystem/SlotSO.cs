using UnityEditor;
using UnityEngine;

namespace SlotSystem
{
    [CreateAssetMenu(fileName = "SlotSO", menuName = "Slot System/Slot")]
    public class SlotSO : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _icon;
        [SerializeField] private SlotType _slotType;
        [SerializeField] private SlotRewardType _rewardType;


        public string GetName()
        {
            return _name;
        }
        
        public string GetDescription()
        {
            return _description;
        }
        
        public Sprite GetIcon()
        {
            return _icon;
        }
        
        public SlotType GetSlotType()
        {
            return _slotType;
        }
        
        public SlotRewardType GetRewardType()
        {
            return _rewardType;
        }
        
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(_name)) return;

            var path = AssetDatabase.GetAssetPath(this);
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.RenameAsset(path, _name + "SlotSO");
            }
        }
#endif
    }
}