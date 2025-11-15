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
            SetupSlots(55);
        }
        
        public void SetupSlots(int zone)
        {
            if (_slotArray == null || _slotArray.Length == 0)
            {
                Debug.LogWarning("SetupSlots called but _slotArray is empty.", this);
                return;
            }

            // Fill all slots with non-bomb rewards based on power
            SetupSlotWithReward(zone);

            // Guarantee exactly one bomb slot
            AssignRandomBombSlot();
        }

        private void AssignRandomBombSlot()
        {
            if (_bombSlotSOArray != null && _bombSlotSOArray.Length > 0)
            {
                var bombIndex = UnityEngine.Random.Range(0, _slotArray.Length);
                var bombSo = GetRandomSlotSOFromArray(_bombSlotSOArray);
                if (bombSo != null)
                {
                    _slotArray[bombIndex].SetSlot(bombSo, 0);
                }
            }
            else
            {
                Debug.LogWarning("SetupSlots: _bombSlotSOArray is empty, cannot assign bomb slot.", this);
            }
        }

        private void SetupSlotWithReward(int power)
        {
            for (var i = 0; i < _slotArray.Length; i++)
            {
                var rewardType = GetRandomNonBombRewardTypeForPower(power);
                var slotSo = GetRandomSlotSOForRewardType(rewardType);
                if (slotSo == null)
                {
                    Debug.LogWarning($"No SlotSO found for reward type {rewardType}.", this);
                    continue;
                }

                var count = GetRandomCountForPower(power, rewardType);
                _slotArray[i].SetSlot(slotSo, count);
            }
        }

        private SlotRewardType GetRandomNonBombRewardTypeForPower(int power)
        {
            var t = Mathf.Clamp01(power / 100f);

            var commonWeight = Mathf.Lerp(60f, 5f, t);
            var rareWeight = Mathf.Lerp(30f, 20f, t);
            var epicWeight = Mathf.Lerp(9f, 35f, t);
            var legendaryWeight = Mathf.Lerp(1f, 40f, t);

            var total = commonWeight + rareWeight + epicWeight + legendaryWeight;
            var weightedRandom = UnityEngine.Random.Range(0f, total);

            if (weightedRandom < commonWeight) return SlotRewardType.RewardCommon;
            
            weightedRandom -= commonWeight;

            if (weightedRandom < rareWeight) return SlotRewardType.RewardRare;
            
            weightedRandom -= rareWeight;

            if (weightedRandom < epicWeight) return SlotRewardType.RewardEpic;

            return SlotRewardType.RewardLegendary;
        }

        private SlotSO GetRandomSlotSOForRewardType(SlotRewardType type)
        {
            return type switch
            {
                SlotRewardType.RewardCommon => GetRandomSlotSOFromArray(_commonSlotSOArray),
                SlotRewardType.RewardRare => GetRandomSlotSOFromArray(_rareSlotSOArray),
                SlotRewardType.RewardEpic => GetRandomSlotSOFromArray(_epicSlotSOArray),
                SlotRewardType.RewardLegendary => GetRandomSlotSOFromArray(_legendarySlotSOArray),
                var _ => throw new ArgumentOutOfRangeException(">" + type + "<", "Unhandled SlotRewardType value.")
            };
        }

        private SlotSO GetRandomSlotSOFromArray(SlotSO[] array)
        {
            if (array == null || array.Length == 0)
                return null;

            var index = UnityEngine.Random.Range(0, array.Length);
            return array[index];
        }

        private int GetRandomCountForPower(int power, SlotRewardType type)
        {
            var t = Mathf.Clamp01(power / 100f);

            var minCount = 1;
            var maxCount = Mathf.RoundToInt(Mathf.Lerp(2f, 10f, t));

            switch (type)
            {
                case SlotRewardType.RewardCommon:
                    maxCount += 1;
                    break;
                case SlotRewardType.RewardLegendary:
                    maxCount = Mathf.Max(2, maxCount - 2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(">" + type + "<", "Unhandled SlotRewardType value.");
            }

            maxCount = Mathf.Max(minCount, maxCount);
            return UnityEngine.Random.Range(minCount, maxCount + 1);
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