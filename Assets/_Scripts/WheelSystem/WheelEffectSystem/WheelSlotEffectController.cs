using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using ObjectPoolSystem;
using UnityEngine;

namespace WheelSystem.WheelEffectSystem
{
    public class WheelSlotEffectController : MonoBehaviour
    {
        [Header("Pooling")]
        [SerializeField] private ObjectPoolManagerUI _objectPoolManager;
        [SerializeField] private ObjectPoolType _objectPoolType = ObjectPoolType.None;

        [Header("UI References")]
        [SerializeField] private RectTransform _effectParent;

        [Header("Effect Settings")]
        [SerializeField] private float _scatterRadius = 40f;
        [SerializeField] private float _scatterDuration = 0.2f;
        [SerializeField] private float _moveDuration = 0.4f;
        
        
        public void PlayAsync(int spawnCount, RectTransform spawnTransform, RectTransform targetTransform, Sprite slotSprite)
        {
            PlayInternalAsync(spawnCount, spawnTransform, targetTransform, slotSprite);
        }

        public float GetMoveDuration()
        {
            return _moveDuration;
        }

        private void PlayInternalAsync(int spawnCount, RectTransform spawnTransform, RectTransform targetTransform, Sprite slotSprite)
        {
            for (var i = 0; i < spawnCount; i++)
            {
                SpawnSingleEffect(spawnTransform, targetTransform, slotSprite);
            }
        }

        private void SpawnSingleEffect(RectTransform spawnTransform, RectTransform targetTransform, Sprite slotSprite)
        {
            var instance = _objectPoolManager.Spawn(_objectPoolType, Vector3.zero, Quaternion.identity);
            if (instance == null)
                return;
            
            if (!instance.TryGetComponent<WheelSlotEffect>(out var wheelSlotEffect)) return;
            
            wheelSlotEffect.SetImage(slotSprite);
            
            var parentRect = _effectParent != null 
                ? _effectParent 
                : spawnTransform.parent as RectTransform;

            if (parentRect == null)
            {
                Debug.LogError("[WheelSlotEffectController] Effect parent RectTransform değil.", this);
                return;
            }

            wheelSlotEffect.GetRectTransform().SetParent(parentRect, worldPositionStays: false);

            var spawnLocal = WorldToLocalInParent(parentRect, spawnTransform);
            var targetLocal = WorldToLocalInParent(parentRect, targetTransform);

            wheelSlotEffect.GetRectTransform().anchoredPosition = spawnLocal;

            var randomOffset = UnityEngine.Random.insideUnitCircle * _scatterRadius;
            var scatterPos = spawnLocal + randomOffset;

            var sequence = DOTween.Sequence();
            sequence.Append(
                wheelSlotEffect.GetRectTransform().DOAnchorPos(scatterPos, _scatterDuration)
                    .SetEase(Ease.OutQuad)
            );
            sequence.Append(
                wheelSlotEffect.GetRectTransform().DOAnchorPos(targetLocal, _moveDuration)
                    .SetEase(Ease.InQuad)
            );
            sequence.Append(
                wheelSlotEffect.GetImage().DOFade(0f, 0.25f)
                    .SetEase(Ease.OutQuad).OnComplete(() =>
                    {
                        var color = wheelSlotEffect.GetImage().color;
                        color.a = 1;
                        wheelSlotEffect.GetImage().color = color;
                    })
            );

            var totalDuration = _scatterDuration + _moveDuration;
            _objectPoolManager.DespawnAfter(_objectPoolType, instance, totalDuration + 0.05f)
                .Forget();
        }

        private static Vector2 WorldToLocalInParent(RectTransform parent, RectTransform child)
        {
            var screenPos = RectTransformUtility.WorldToScreenPoint(null, child.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPos, null, out var localPos);
            return localPos;
        }    }
}