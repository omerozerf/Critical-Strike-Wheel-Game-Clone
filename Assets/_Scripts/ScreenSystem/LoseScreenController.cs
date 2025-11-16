using System;
using DG.Tweening;
using UnityEngine;
using WheelSystem;

namespace ScreenSystem
{
    public class LoseScreenController : ScreenController
    {
        [SerializeField] private Transform[] _redGlowTransformArray;
        [SerializeField] private float _redGlowDuration;
        [SerializeField] private float _redGlowScale;

        private Tween[] m_RedGlowTweens;


        private void Awake()
        {
            WheelSlotSelector.OnBombSelected += HandleBombSelected;
        }

        private void HandleBombSelected()
        {
            Show();
        }

        private void OnValidate()
        {
            InitializeCanvasGroup();
        }

        
        private void InitializeCanvasGroup()
        {
            if (!_canvasGroup)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }
        }
        
        private void ShowRedGlow()
        {
            m_RedGlowTweens = new Tween[_redGlowTransformArray.Length];

            for (var i = 0; i < _redGlowTransformArray.Length; i++)
            {
                var redGlow = _redGlowTransformArray[i];
                m_RedGlowTweens[i] = redGlow.transform
                    .DOScale(_redGlowScale, _redGlowDuration)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            }
        }

        private void StopRedGlow()
        {
            if (m_RedGlowTweens == null) return;

            foreach (var t in m_RedGlowTweens)
            {
                t?.Kill();
            }
        }

        public override void Show()
        {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.DOFade(1f, 0.5f).SetEase(Ease.InOutSine);
            ShowRedGlow();
        }

        public override void Hide()
        {
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.DOFade(0f, 0.5f).SetEase(Ease.InOutSine);
            StopRedGlow();
        }
    }
}