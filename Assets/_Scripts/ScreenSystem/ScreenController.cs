using System;
using UnityEngine;

namespace ScreenSystem
{
    public abstract class ScreenController : MonoBehaviour
    {
        [SerializeField] protected CanvasGroup _canvasGroup;


        private void OnValidate()
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