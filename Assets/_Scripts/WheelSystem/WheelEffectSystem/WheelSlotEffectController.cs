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
        [SerializeField] private RectTransform _spawnRoot;   // Soldaki WheelSlotEffect referansı
        [SerializeField] private RectTransform _target;      // Sağdaki target (gideceği yer)
        [SerializeField] private RectTransform _effectParent; // Effectlerin parent'ı (Canvas içi)

        [Header("Effect Settings")]
        [SerializeField] private int _defaultSpawnCount = 5;
        [SerializeField] private float _scatterRadius = 40f;
        [SerializeField] private float _scatterDuration = 0.2f;
        [SerializeField] private float _moveDuration = 0.4f;
        [SerializeField] private float _betweenSpawnDelay = 0.03f;

        /// <summary>
        /// Default sayıda efekt oynat.
        /// </summary>
        public UniTask PlayAsync()
        {
            return PlayInternalAsync(_defaultSpawnCount);
        }

        private void Start()
        {
            Invoke(nameof(PlayAsync), 3f);
        }

        /// <summary>
        /// Dışarıdan kaç adet obje çağırılacağı verilir.
        /// </summary>
        public UniTask PlayAsync(int spawnCount)
        {
            return PlayInternalAsync(spawnCount);
        }

        private async UniTask PlayInternalAsync(int spawnCount)
        {
            if (_objectPoolManager == null || _spawnRoot == null || _target == null)
            {
                Debug.LogWarning("[WheelSlotEffectController] Referanslar eksik.", this);
                return;
            }

            if (_objectPoolType == ObjectPoolType.None)
            {
                Debug.LogWarning("[WheelSlotEffectController] ObjectPoolType.None kullanılamaz.", this);
                return;
            }

            if (spawnCount <= 0)
                return;

            for (int i = 0; i < spawnCount; i++)
            {
                SpawnSingleEffect();

                if (_betweenSpawnDelay > 0f && i < spawnCount - 1)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(_betweenSpawnDelay));
                }
            }
        }

        private void SpawnSingleEffect()
        {
            // Pool'dan obje al
            var instance = _objectPoolManager.Spawn(_objectPoolType, Vector3.zero, Quaternion.identity);
            if (instance == null)
                return;

            if (!instance.TryGetComponent<RectTransform>(out var rect))
            {
                Debug.LogError("[WheelSlotEffectController] Pooled obje RectTransform içermiyor.", instance);
                return;
            }

            // Parent'ı ayarla (Canvas hiyerarşisi bozulmasın)
            if (_effectParent != null)
            {
                rect.SetParent(_effectParent, worldPositionStays: false);
            }
            else
            {
                rect.SetParent(_spawnRoot.parent, worldPositionStays: false);
            }

            // Başlangıç anchoredPos: soldaki WheelSlotEffect pozisyonu
            rect.anchoredPosition = _spawnRoot.anchoredPosition;

            // Scatter için rastgele offset
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * _scatterRadius;
            Vector2 scatterPos = _spawnRoot.anchoredPosition + randomOffset;

            // Hedef anchoredPos: target
            Vector2 targetPos = _target.anchoredPosition;

            // Dotween sequence: önce etrafa saçıl, sonra target'a git
            var sequence = DOTween.Sequence();
            sequence.Append(
                rect.DOAnchorPos(scatterPos, _scatterDuration)
                    .SetEase(Ease.OutQuad)
            );
            sequence.Append(
                rect.DOAnchorPos(targetPos, _moveDuration)
                    .SetEase(Ease.InQuad)
            );

            // Animasyonlar bitince otomatik havuza dönsün
            float totalDuration = _scatterDuration + _moveDuration;
            _objectPoolManager.DespawnAfter(_objectPoolType, instance, totalDuration + 0.05f);
        }
    }
}