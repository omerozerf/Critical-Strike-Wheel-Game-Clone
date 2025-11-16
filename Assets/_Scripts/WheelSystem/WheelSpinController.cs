using System;
using DG.Tweening;
using UnityEngine;

namespace WheelSystem
{
    public class WheelSpinController : MonoBehaviour
    {
        [SerializeField] private Transform _spinTransform;
        [SerializeField] private float _spinTime;
        [SerializeField] private float _fixSpinTime;
        [SerializeField] private int _minFullTurns;
        [SerializeField] private int _maxFullTurns;

        public static event Action<int> OnWheelStopped; 
        
        private bool m_IsSpinning;
        
        
        private void Awake()
        {
            SpinButton.OnButtonClicked += HandleSpinButtonClicked;
            WheelSlotController.OnSlotsChanged += HandleSlotsChanged;
            m_IsSpinning = true;
        }

        private void OnDestroy()
        {
            SpinButton.OnButtonClicked -= HandleSpinButtonClicked;
            WheelSlotController.OnSlotsChanged -= HandleSlotsChanged;
        }
        
        private void OnValidate()
        {
            InitializeSpinTransform();
        }
        
        
        private void HandleSlotsChanged()
        {
            m_IsSpinning = false;
        }

        private void HandleSpinButtonClicked()
        {
            if (m_IsSpinning) return;

            m_IsSpinning = true;

            SpinWheel();
        }

        private void SpinWheel()
        {
            var fullTurns = UnityEngine.Random.Range(_minFullTurns, _maxFullTurns + 1);
            var baseAngle = fullTurns * 360f;
            var finalAngle = baseAngle + UnityEngine.Random.Range(0f, 360f);

            _spinTransform
                .DORotate(new Vector3(0f, 0f, -finalAngle), _spinTime, RotateMode.FastBeyond360)
                .SetEase(Ease.OutQuart)
                .OnComplete(FixTargetAngle);
        }

        private void FixTargetAngle()
        {
            var finalZ = _spinTransform.eulerAngles.z;
            finalZ = (finalZ + 360f) % 360f;

            var sliceCount = 8;
            var sliceSize = 360f / sliceCount;

            var adjustedAngle = (360f - finalZ) % 360f;

            var nearestSlice = Mathf.RoundToInt(adjustedAngle / sliceSize) % sliceCount;

            var targetCenterAngle = nearestSlice * sliceSize;
            var targetZ = (360f - targetCenterAngle) % 360f;

            _spinTransform
                .DORotate(new Vector3(0f, 0f, targetZ), _fixSpinTime)
                .SetEase(Ease.OutBack, 0.8f)
                .OnComplete(() => {
                    {
                        var stoppedSlice = Mathf.RoundToInt(_spinTransform.eulerAngles.z / (360f / sliceCount));
                                                                                
                        OnWheelStopped?.Invoke(stoppedSlice);
                    }
                });
        }

        private void InitializeSpinTransform()
        {
            if (!_spinTransform)
            {
                _spinTransform = GetComponent<Transform>();
            }
        }
    }
}