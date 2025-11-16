using System;
using UnityEngine;
using UnityEngine.UI;

namespace ButtonSystem
{
    public class SpinButton : MonoBehaviour
    {
        [SerializeField] private Button _spinButton;
    
        public static event Action OnButtonClicked;
    
    
        private void Awake()
        {
            _spinButton.onClick.AddListener(HandleSpinButtonClicked);
        }

        private void OnDestroy()
        {
            _spinButton.onClick.RemoveListener(HandleSpinButtonClicked);
        }
    
        private void OnValidate()
        {
            InitializeSpinButton();
        }
    
    
        private void HandleSpinButtonClicked()
        {
            OnButtonClicked?.Invoke();
        }

    
        private void InitializeSpinButton()
        {
            if (!_spinButton)
            {
                _spinButton = GetComponent<Button>();
            }
        }
    }
}
