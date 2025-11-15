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

        private void Start()
        {
            SetupSlots(5);
        }


        private void HandleWheelStopped(int slotIndex)
        {
            SetupSlots(55);
        }
        
        public void SetupSlots(int power)
        {
            if (_slotArray == null || _slotArray.Length == 0)
            {
                Debug.LogWarning("SetupSlots called but _slotArray is empty.", this);
                return;
            }

            // Fill all slots with non-bomb rewards based on power
            for (int i = 0; i < _slotArray.Length; i++)
            {
                var rewardType = GetRandomNonBombRewardTypeForPower(power);
                var slotSo = GetRandomSlotSOForRewardType(rewardType);
                if (slotSo == null)
                {
                    Debug.LogWarning($"No SlotSO found for reward type {rewardType}.", this);
                    continue;
                }

                int count = GetRandomCountForPower(power, rewardType);
                _slotArray[i].SetSlot(slotSo, count);
            }

            // Guarantee exactly one bomb slot
            if (_bombSlotSOArray != null && _bombSlotSOArray.Length > 0)
            {
                int bombIndex = UnityEngine.Random.Range(0, _slotArray.Length);
                var bombSo = GetRandomSlotSOFromArray(_bombSlotSOArray);
                if (bombSo != null)
                {
                    // Bomb için count sayısı UI'da gösterilmeyecek, Slot tarafında Bomb için text'i boş bırakabilirsin.
                    _slotArray[bombIndex].SetSlot(bombSo, 0);
                }
            }
            else
            {
                Debug.LogWarning("SetupSlots: _bombSlotSOArray is empty, cannot assign bomb slot.", this);
            }
        }

        private SlotRewardType GetRandomNonBombRewardTypeForPower(int power)
        {
            // power 0–100 aralığına normalize edilir, üstü 100 kabul edilir
            float t = Mathf.Clamp01(power / 100f);

            // Ağırlıklar: power düşükken common baskın, yüksekken legendary baskın
            float commonWeight = Mathf.Lerp(60f, 5f, t);
            float rareWeight = Mathf.Lerp(30f, 20f, t);
            float epicWeight = Mathf.Lerp(9f, 35f, t);
            float legendaryWeight = Mathf.Lerp(1f, 40f, t);

            float total = commonWeight + rareWeight + epicWeight + legendaryWeight;
            float r = UnityEngine.Random.Range(0f, total);

            if (r < commonWeight)
                return SlotRewardType.RewardCommon;
            r -= commonWeight;

            if (r < rareWeight)
                return SlotRewardType.RewardRare;
            r -= rareWeight;

            if (r < epicWeight)
                return SlotRewardType.RewardEpic;

            return SlotRewardType.RewardLegendary;
        }

        private SlotSO GetRandomSlotSOForRewardType(SlotRewardType type)
        {
            switch (type)
            {
                case SlotRewardType.RewardCommon:
                    return GetRandomSlotSOFromArray(_commonSlotSOArray);
                case SlotRewardType.RewardRare:
                    return GetRandomSlotSOFromArray(_rareSlotSOArray);
                case SlotRewardType.RewardEpic:
                    return GetRandomSlotSOFromArray(_epicSlotSOArray);
                case SlotRewardType.RewardLegendary:
                    return GetRandomSlotSOFromArray(_legendarySlotSOArray);
                default:
                    return null;
            }
        }

        private SlotSO GetRandomSlotSOFromArray(SlotSO[] array)
        {
            if (array == null || array.Length == 0)
                return null;

            int index = UnityEngine.Random.Range(0, array.Length);
            return array[index];
        }

        private int GetRandomCountForPower(int power, SlotRewardType type)
        {
            // power 0–100 aralığına normalize edilir, üstü 100 kabul edilir
            float t = Mathf.Clamp01(power / 100f);

            int minCount = 1;
            int maxCount = Mathf.RoundToInt(Mathf.Lerp(2f, 10f, t));

            // İstersen rarity'ye göre ufak ayar yap
            switch (type)
            {
                case SlotRewardType.RewardCommon:
                    // common'lar biraz daha yüksek stack alabilir
                    maxCount += 1;
                    break;
                case SlotRewardType.RewardLegendary:
                    // legendary'ler daha düşük stack ile daha değerli olabilir
                    maxCount = Mathf.Max(2, maxCount - 2);
                    break;
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