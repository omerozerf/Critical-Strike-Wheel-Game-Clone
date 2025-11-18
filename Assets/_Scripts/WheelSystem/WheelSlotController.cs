using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Managers;
using SlotSystem;
using UnityEngine;
using WheelSystem.WheelSlotControllerHelpers;
using ZoneSystem;

namespace WheelSystem
{
    public class WheelSlotController : MonoBehaviour
    {
        [Header("Slot Editor")]
        [SerializeField] private SliceEditorConfig[] _sliceEditorConfigs;

        [Space]
        [Header("Slot Animation")]
        [SerializeField] private float _slotCountAnimationDuration;

        [Space]
        [Header("Slot Components")]
        [SerializeField] private Slot[] _slotArray;
        [SerializeField] private SlotSO[] _allSlotSOArray;

        public static event Action OnSlotsChanged;

        private SlotLibrary m_SlotLibrary;
        private WheelRewardCalculator m_RewardCalculator;

        

        private void Awake()
        {
            InitServices();

            ZonePanelController.OnZoneChanged += HandleOnZoneChanged;
        }

        private void OnDestroy()
        {
            ZonePanelController.OnZoneChanged -= HandleOnZoneChanged;
        }

        private void OnValidate()
        {
            ValidateSlotComponents();

            if (m_SlotLibrary == null)
                m_SlotLibrary = new SlotLibrary();

            m_SlotLibrary.CategorizeFrom(_allSlotSOArray);
        }



        private void InitServices()
        {
            if (m_SlotLibrary == null)
                m_SlotLibrary = new SlotLibrary();

            m_SlotLibrary.CategorizeFrom(_allSlotSOArray);

            if (m_RewardCalculator == null)
            {
                m_RewardCalculator = new WheelRewardCalculator(
                    GameCommonVariableManager.GetSafeZoneInterval,
                    GameCommonVariableManager.GetSuperZoneInterval);
            }
        }



        private void HandleOnZoneChanged(int zoneNumber)
        {
            var fixedZone = zoneNumber + 1; // 1-based zone
            SetupSlots(fixedZone);
        }



        private async void SetupSlots(int zone)
        {
            if (!HasSlots())
                return;

            await AnimateSlotsHide();

            FillSlotsForZone(zone);

            AssignRandomBombSlot(zone);

            await AnimateSlotsReveal();

            OnSlotsChanged?.Invoke();
        }

        private bool HasSlots()
        {
            if (_slotArray == null || _slotArray.Length == 0)
            {
                Debug.LogWarning("SetupSlots called but _slotArray is empty.", this);
                return false;
            }

            return true;
        }

        private async UniTask AnimateSlotsHide()
        {
            foreach (var slot in _slotArray)
            {
                var t = slot.transform;
                t.DORotate(
                    new Vector3(t.eulerAngles.x, 90f, t.eulerAngles.z),
                    _slotCountAnimationDuration);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(_slotCountAnimationDuration));
        }

        private async UniTask AnimateSlotsReveal()
        {
            foreach (var slot in _slotArray)
            {
                var t = slot.transform;
                t.DORotate(
                    new Vector3(t.eulerAngles.x, 0f, t.eulerAngles.z),
                    _slotCountAnimationDuration);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(_slotCountAnimationDuration));
        }

        private void FillSlotsForZone(int zone)
        {
            for (var i = 0; i < _slotArray.Length; i++)
            {
                var rewardType = GetRandomRewardTypeForZoneAndSlice(zone, i);
                var slotSo = GetRandomSlotSOForRewardTypeAndSlice(rewardType, i);

                if (slotSo == null)
                {
                    Debug.LogWarning($"No SlotSO found for reward type {rewardType} on slice {i}.", this);
                    continue;
                }

                var count = m_RewardCalculator.GetRandomCountForZoneAndType(zone, rewardType);
                _slotArray[i].SetSlot(slotSo, count);
            }
        }



        private SlotRewardType GetRandomRewardTypeForZone(int zone)
        {
            return m_RewardCalculator.GetRandomNonBombRewardTypeForZone(zone);
        }

        private SlotRewardType GetRandomRewardTypeForZoneAndSlice(int zone, int sliceIndex)
        {
            var weights = m_RewardCalculator.GetBaseWeightsForZone(zone);

            ApplySliceConstraints(sliceIndex, ref weights);

            var total = weights.common + weights.rare + weights.epic + weights.legendary;
            if (total <= 0.0001f)
            {
                return GetRandomRewardTypeForZone(zone);
            }

            return m_RewardCalculator.GetRandomNonBombRewardTypeWithWeights(weights);
        }

        private void ApplySliceConstraints(int sliceIndex, ref RewardWeights w)
        {
            if (!SliceAllowsRewardType(sliceIndex, SlotRewardType.RewardCommon))    w.common = 0f;
            if (!SliceAllowsRewardType(sliceIndex, SlotRewardType.RewardRare))      w.rare = 0f;
            if (!SliceAllowsRewardType(sliceIndex, SlotRewardType.RewardEpic))      w.epic = 0f;
            if (!SliceAllowsRewardType(sliceIndex, SlotRewardType.RewardLegendary)) w.legendary = 0f;
        }

        private bool SliceAllowsRewardType(int sliceIndex, SlotRewardType type)
        {
            if (_sliceEditorConfigs == null ||
                sliceIndex < 0 ||
                sliceIndex >= _sliceEditorConfigs.Length)
                return true;

            var cfg = _sliceEditorConfigs[sliceIndex];
            if (cfg == null || cfg._allowedSlots == null || cfg._allowedSlots.Length == 0)
                return true;

            foreach (var so in cfg._allowedSlots)
            {
                if (so == null) continue;
                if (so.GetRewardType() == type)
                    return true;
            }

            return false;
        }



        private SlotSO GetRandomSlotSOForRewardTypeAndSlice(SlotRewardType type, int sliceIndex)
        {
            if (_sliceEditorConfigs != null &&
                sliceIndex >= 0 &&
                sliceIndex < _sliceEditorConfigs.Length)
            {
                var cfg = _sliceEditorConfigs[sliceIndex];
                if (cfg != null && cfg._allowedSlots != null && cfg._allowedSlots.Length > 0)
                {
                    var candidate = m_SlotLibrary.GetRandomFromAllowed(type, cfg._allowedSlots);
                    if (candidate != null)
                        return candidate;
                }
            }

            return m_SlotLibrary.GetRandomForRewardType(type);
        }


        #region Bomb Assignment

        private void AssignRandomBombSlot(int zone)
        {
            if (m_RewardCalculator.IsSafeZone(zone) || m_RewardCalculator.IsSuperZone(zone))
                return;

            var bombSo = m_SlotLibrary.GetRandomForRewardType(SlotRewardType.Bomb);
            if (bombSo == null)
            {
                Debug.LogWarning("AssignRandomBombSlot: no bomb SlotSO found in SlotLibrary.", this);
                return;
            }

            var bombIndex = UnityEngine.Random.Range(0, _slotArray.Length);
            _slotArray[bombIndex].SetSlot(bombSo, 0);
        }

        #endregion

        #region Validation

        private void ValidateSlotComponents()
        {
            if (_slotArray == null || _slotArray.Length == 0)
            {
                _slotArray = GetComponentsInChildren<Slot>();
                if (_slotArray.Length != 8)
                {
                    Debug.LogWarning("WheelSlotController expects exactly 8 Slot components as children.", this);
                }
            }
        }

        #endregion
    }
}