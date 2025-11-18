using System;
using UnityEngine;

namespace ScreenSystem
{
    public abstract class ScreenController : MonoBehaviour
    {
        [SerializeField] protected float _fadeInDuration = 0.5f;
        [SerializeField] protected CanvasGroup _canvasGroup;


        protected virtual void OnValidate()
        {
            InitializeCanvasGroup();
        }

        
        public abstract void Show();
        public abstract void Hide();
        
        
        private void InitializeCanvasGroup()
        {
            if (!_canvasGroup)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }
        }
    }
}