using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SlotSystem;
using UnityEngine;
using ZoneSystem;

namespace WheelSystem
{
    public class WheelSlotController : MonoBehaviour
    {
        [SerializeField] private float _slotCountAnimationDuration;
        [SerializeField] private Slot[] _slotArray;
        [SerializeField] private SlotSO[] _allSlotSOArray;
        [SerializeField] private SlotSO[] _commonSlotSOArray;
        [SerializeField] private SlotSO[] _rareSlotSOArray;
        [SerializeField] private SlotSO[] _epicSlotSOArray;
        [SerializeField] private SlotSO[] _legendarySlotSOArray;
        [SerializeField] private SlotSO[] _bombSlotSOArray;


        private void Awake()
        {
            ZonePanelController.OnZoneChanged += HandleOnZoneChanged;
        }
        
        private void OnValidate()
        {
            ValidateSlotComponents();
            CategorizeSlotRewards();
        }
        
        
        private void HandleOnZoneChanged(int zoneNumber)
        {
            var fixZone = zoneNumber + 1; // Fix zone number to be 1-based
            SetupSlots(fixZone);
        }
        

        private bool IsSafeZone(int zone)
        {
            if (GameCommonVariableManager.GetSafeZoneInterval() <= 0) return false;
            return zone == 1 || zone % GameCommonVariableManager.GetSafeZoneInterval() == 0;
        }

        private bool IsSuperZone(int zone)
        {
            if (GameCommonVariableManager.GetSuperZoneInterval() <= 0) return false;
            return zone != 1 && zone % GameCommonVariableManager.GetSuperZoneInterval() == 0;
        }
        
        private async void SetupSlots(int zone)
        {
            if (_slotArray == null || _slotArray.Length == 0)
            {
                Debug.LogWarning("SetupSlots called but _slotArray is empty.", this);
                return;
            }

            foreach (var slot in _slotArray)
            {
                slot.transform
                    .DORotate(
                        new Vector3(slot.transform.eulerAngles.x, 90f, slot.transform.eulerAngles.z), 
                        _slotCountAnimationDuration);
            }

            await UniTask.WaitForSeconds(_slotCountAnimationDuration);

            // Fill all slots with non-bomb rewards based on power
            SetupSlotWithReward(zone);

            AssignRandomBombSlot(zone);
            
            foreach (var slot in _slotArray)
            {
                slot.transform
                    .DORotate(new Vector3(slot.transform.eulerAngles.x, 0f, slot.transform.eulerAngles.z),
                        _slotCountAnimationDuration);
            }
        }

        private void AssignRandomBombSlot(int zone)
        {
            if (zone == 1 || zone % GameCommonVariableManager.GetSafeZoneInterval() == 0 ||
                zone % GameCommonVariableManager.GetSuperZoneInterval() == 0) return;
            
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

            float commonWeight;
            float rareWeight;
            float epicWeight;
            float legendaryWeight;

            if (IsSuperZone(power))
            {
                // Super zone: mostly Epic/Legendary
                commonWeight = 0f;
                rareWeight = Mathf.Lerp(20f, 10f, t);
                epicWeight = Mathf.Lerp(40f, 45f, t);
                legendaryWeight = Mathf.Lerp(40f, 45f, t);
            }
            else if (IsSafeZone(power))
            {
                // Safe zone: significantly increased high–value rewards
                commonWeight = Mathf.Lerp(30f, 5f, t);
                rareWeight = Mathf.Lerp(35f, 25f, t);
                epicWeight = Mathf.Lerp(25f, 35f, t);
                legendaryWeight = Mathf.Lerp(10f, 35f, t);
            }
            else
            {
                // Normal zone: baseline distribution
                commonWeight = Mathf.Lerp(60f, 5f, t);
                rareWeight = Mathf.Lerp(30f, 20f, t);
                epicWeight = Mathf.Lerp(9f, 35f, t);
                legendaryWeight = Mathf.Lerp(1f, 40f, t);
            }

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

            var isSafe = IsSafeZone(power);
            var isSuper = IsSuperZone(power);

            switch (type)
            {
                case SlotRewardType.RewardCommon:
                    // Common can still appear in decent amounts, but we don't buff it in safe/super.
                    maxCount += 1;
                    break;

                case SlotRewardType.RewardRare:
                    if (isSafe) maxCount += 1;
                    if (isSuper)
                    {
                        minCount = 2;
                        maxCount += 2;
                    }
                    break;

                case SlotRewardType.RewardEpic:
                    maxCount = Mathf.Max(minCount, maxCount - 1);
                    if (isSafe) maxCount += 1;
                    if (isSuper)
                    {
                        minCount = 2;
                        maxCount += 2;
                    }
                    break;

                case SlotRewardType.RewardLegendary:
                    maxCount = Mathf.Max(2, maxCount - 2);
                    if (isSafe) maxCount += 1;
                    if (isSuper)
                    {
                        minCount = 2;
                        maxCount += 2;
                    }
                    break;

                case SlotRewardType.Bomb:
                    return 0;

                case SlotRewardType.None:
                default:
                    throw new ArgumentOutOfRangeException(">" + type + "<", "Unhandled SlotRewardType value.");
            }

            maxCount = Mathf.Max(minCount, maxCount);
            return UnityEngine.Random.Range(minCount, maxCount + 1);
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

            var commons = new List<SlotSO>();
            var rares = new List<SlotSO>();
            var epics = new List<SlotSO>();
            var legendaries = new List<SlotSO>();
            var bombs = new List<SlotSO>();

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