using System;
using System.Collections.Generic;
using SlotSystem;

namespace WheelSystem.WheelSlotControllerHelpers
{
    internal sealed class SlotLibrary
    {
        private SlotSO[] CommonSlots { get; set; } = Array.Empty<SlotSO>();
        private SlotSO[] RareSlots { get; set; } = Array.Empty<SlotSO>();
        private SlotSO[] EpicSlots { get; set; } = Array.Empty<SlotSO>();
        private SlotSO[] LegendarySlots { get; set; } = Array.Empty<SlotSO>();
        private SlotSO[] BombSlots { get; set; } = Array.Empty<SlotSO>();

        public void CategorizeFrom(SlotSO[] allSlots)
        {
            if (allSlots == null || allSlots.Length == 0)
            {
                CommonSlots = RareSlots = EpicSlots = LegendarySlots = BombSlots = Array.Empty<SlotSO>();
                return;
            }

            var commons = new List<SlotSO>();
            var rares = new List<SlotSO>();
            var epics = new List<SlotSO>();
            var legendaries = new List<SlotSO>();
            var bombs = new List<SlotSO>();

            foreach (var so in allSlots)
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
                        throw new ArgumentOutOfRangeException("SlotRewardType.None is not a valid reward type for SlotSO.");
                    default:
                        throw new ArgumentOutOfRangeException("Unhandled SlotRewardType value.");
                }
            }

            CommonSlots = commons.ToArray();
            RareSlots = rares.ToArray();
            EpicSlots = epics.ToArray();
            LegendarySlots = legendaries.ToArray();
            BombSlots = bombs.ToArray();
        }

        public SlotSO GetRandomForRewardType(SlotRewardType type)
        {
            SlotSO[] array = type switch
            {
                SlotRewardType.RewardCommon => CommonSlots,
                SlotRewardType.RewardRare => RareSlots,
                SlotRewardType.RewardEpic => EpicSlots,
                SlotRewardType.RewardLegendary => LegendarySlots,
                SlotRewardType.Bomb => BombSlots,
                _ => throw new ArgumentOutOfRangeException(">" + type + "<", "Unhandled SlotRewardType value.")
            };

            if (array == null || array.Length == 0)
                return null;

            var index = UnityEngine.Random.Range(0, array.Length);
            return array[index];
        }

        public SlotSO GetRandomFromAllowed(SlotRewardType type, SlotSO[] allowedSlots)
        {
            if (allowedSlots == null || allowedSlots.Length == 0)
                return null;

            var candidates = new List<SlotSO>();

            foreach (var so in allowedSlots)
            {
                if (so == null) continue;
                if (so.GetRewardType() != type) continue;

                candidates.Add(so);
            }

            if (candidates.Count == 0)
                return null;

            var index = UnityEngine.Random.Range(0, candidates.Count);
            return candidates[index];
        }
    }
}