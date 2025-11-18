using ScreenSystem;
using UnityEngine;
using UnityEngine.UI;
using WheelSystem;

namespace ButtonSystem
{
    public class ExitButton : MonoBehaviour
    {
        [SerializeField] private Button _exitButton;
        [SerializeField] private ScreenController _screenController;
        
        private void Awake()
        {
            SpinButton.OnButtonClicked += HandleSpinButtonClicked;
            WheelSlotController.OnSlotsChanged += HandleSlotsChanged;
            _exitButton.onClick.AddListener(HandleExitButtonClicked);
        }
        
        private void OnDestroy()
        {
            SpinButton.OnButtonClicked -= HandleSpinButtonClicked;
            WheelSlotController.OnSlotsChanged -= HandleSlotsChanged;
            _exitButton.onClick.RemoveListener(HandleExitButtonClicked);
        }
        
        private void OnValidate()
        {
            InitializeExitButton();
        }
        
        
        private void HandleExitButtonClicked()
        {
            _screenController.Show();
        }
        
        private void HandleSlotsChanged()
        {
            _exitButton.interactable = true;
        }

        private void HandleSpinButtonClicked()
        {
            _exitButton.interactable = false;
        }
        
        
        private void InitializeExitButton()
        {
            if (!_exitButton) _exitButton = GetComponent<Button>();
        }
    }
}