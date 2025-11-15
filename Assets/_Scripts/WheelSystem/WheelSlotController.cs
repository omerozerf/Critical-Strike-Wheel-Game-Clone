using System;
using SlotSystem;
using UnityEngine;

namespace WheelSystem
{
    public class WheelSlotController : MonoBehaviour
    {
        [SerializeField] private Slot[] _slotArray;
        [SerializeField] private SlotSO[] _allSlotSOArray;
        [SerializeField] private SlotSO[] _commonSlotSOArray;
        [SerializeField] private SlotSO[] _rareSlotSOArray;
        [SerializeField] private SlotSO[] _epicSlotSOArray;
        [SerializeField] private SlotSO[] _legendarySlotSOArray;
        [SerializeField] private SlotSO[] _bombSlotSOArray;


        private void Awake()
        {
            WheelController.OnWheelStopped += HandleWheelStopped;
        }

        private void OnDestroy()
        {
            WheelController.OnWheelStopped -= HandleWheelStopped;
        }
        

        private void HandleWheelStopped(int slotIndex)
        {
            
        }


        private void OnValidate()
        {
            ValidateSlotComponents();
            CategorizeSlotRewards();
        }

        
        private void ValidateSlotComponents()
        {
            if (_slotArray == null || _slotArray.Length == 0)
            {
                _slotArray = GetComponentsInChildren<Slot>();
                if (_slotArray.Length != 8)
                {
                    Debug.LogWarning("WheelResultController expects exactly 8 Slot components as children.", this);
                }
            }
        }

        private void CategorizeSlotRewards()
        {
            if (_allSlotSOArray == null || _allSlotSOArray.Length == 0)
                return;

            var commons = new System.Collections.Generic.List<SlotSO>();
            var rares = new System.Collections.Generic.List<SlotSO>();
            var epics = new System.Collections.Generic.List<SlotSO>();
            var legendaries = new System.Collections.Generic.List<SlotSO>();
            var bombs = new System.Collections.Generic.List<SlotSO>();

            foreach (var so in _allSlotSOArray)
            {
                if (so == null) continue;

                switch (so.GetRewardType())
                {
                    case SlotRewardType.RewardCommon:
                        commons.Add(so);
                        break;
                    case SlotRewardType.RewardRare:
                        rares.Add(so);
                        break;
                    case SlotRewardType.RewardEpic:
                        epics.Add(so);
                        break;
                    case SlotRewardType.RewardLegendary:
                        legendaries.Add(so);
                        break;
                    case SlotRewardType.Bomb:
                        bombs.Add(so);
                        break;
                    case SlotRewardType.None:
                        throw new ArgumentOutOfRangeException($"SlotRewardType.None is not a valid reward type for SlotSO.");
                    default:
                        throw new ArgumentOutOfRangeException($"Unhandled SlotRewardType value.");
                }
            }

            _commonSlotSOArray = commons.ToArray();
            _rareSlotSOArray = rares.ToArray();
            _epicSlotSOArray = epics.ToArray();
            _legendarySlotSOArray = legendaries.ToArray();
            _bombSlotSOArray = bombs.ToArray();
        }
    }
}