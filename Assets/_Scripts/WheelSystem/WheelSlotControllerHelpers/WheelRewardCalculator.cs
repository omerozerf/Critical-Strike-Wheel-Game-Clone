using System;
using SlotSystem;
using UnityEngine;

namespace WheelSystem.WheelSlotControllerHelpers
{
    internal sealed class WheelRewardCalculator
    {
        private readonly Func<int> m_SafeZoneIntervalGetter;
        private readonly Func<int> m_SuperZoneIntervalGetter;

        public WheelRewardCalculator(
            Func<int> safeZoneIntervalGetter,
            Func<int> superZoneIntervalGetter)
        {
            m_SafeZoneIntervalGetter = safeZoneIntervalGetter;
            m_SuperZoneIntervalGetter = superZoneIntervalGetter;
        }

        public bool IsSafeZone(int zone)
        {
            var interval = m_SafeZoneIntervalGetter?.Invoke() ?? 0;
            if (interval <= 0) return false;
            return zone == 1 || zone % interval == 0;
        }

        public bool IsSuperZone(int zone)
        {
            var interval = m_SuperZoneIntervalGetter?.Invoke() ?? 0;
            if (interval <= 0) return false;
            return zone != 1 && zone % interval == 0;
        }

        public RewardWeights GetBaseWeightsForZone(int zone)
        {
            var t = Mathf.Clamp01(zone / 100f);

            float commonWeight;
            float rareWeight;
            float epicWeight;
            float legendaryWeight;

            if (IsSuperZone(zone))
            {
                commonWeight = 0f;
                rareWeight = Mathf.Lerp(20f, 10f, t);
                epicWeight = Mathf.Lerp(40f, 45f, t);
                legendaryWeight = Mathf.Lerp(40f, 45f, t);
            }
            else if (IsSafeZone(zone))
            {
                commonWeight = Mathf.Lerp(30f, 5f, t);
                rareWeight = Mathf.Lerp(35f, 25f, t);
                epicWeight = Mathf.Lerp(25f, 35f, t);
                legendaryWeight = Mathf.Lerp(10f, 35f, t);
            }
            else
            {
                commonWeight = Mathf.Lerp(60f, 5f, t);
                rareWeight = Mathf.Lerp(30f, 20f, t);
                epicWeight = Mathf.Lerp(9f, 35f, t);
                legendaryWeight = Mathf.Lerp(1f, 40f, t);
            }

            return new RewardWeights
            {
                common = commonWeight,
                rare = rareWeight,
                epic = epicWeight,
                legendary = legendaryWeight
            };
        }

        public SlotRewardType GetRandomNonBombRewardTypeForZone(int zone)
        {
            var weights = GetBaseWeightsForZone(zone);
            return PickRandomRewardType(weights);
        }

        public SlotRewardType GetRandomNonBombRewardTypeWithWeights(RewardWeights weights)
        {
            return PickRandomRewardType(weights);
        }

        public int GetRandomCountForZoneAndType(int zone, SlotRewardType type)
        {
            var t = Mathf.Clamp01(zone / 100f);

            var minCount = 1;
            var maxCount = Mathf.RoundToInt(Mathf.Lerp(2f, 10f, t));

            var isSafe = IsSafeZone(zone);
            var isSuper = IsSuperZone(zone);

            switch (type)
            {
                case SlotRewardType.RewardCommon:
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

        private SlotRewardType PickRandomRewardType(RewardWeights w)
        {
            var total = w.common + w.rare + w.epic + w.legendary;
            if (total <= 0f)
                throw new ArgumentException("Total weight must be > 0.", nameof(w));

            var weightedRandom = UnityEngine.Random.Range(0f, total);

            if (weightedRandom < w.common) return SlotRewardType.RewardCommon;
            weightedRandom -= w.common;

            if (weightedRandom < w.rare) return SlotRewardType.RewardRare;
            weightedRandom -= w.rare;

            if (weightedRandom < w.epic) return SlotRewardType.RewardEpic;

            return SlotRewardType.RewardLegendary;
        }
    }
}