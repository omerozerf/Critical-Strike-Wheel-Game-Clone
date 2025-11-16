using ScreenSystem;
using UnityEngine;
using UnityEngine.UI;

namespace ButtonSystem
{
    public class ExitButton : MonoBehaviour
    {
        [SerializeField] private Button _exitButton;
        [SerializeField] private ScreenController _screenController;
        
        private void Awake()
        {
            _exitButton.onClick.AddListener(HandleExitButtonClicked);
        }
        
        private void OnDestroy()
        {
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
        
        
        private void InitializeExitButton()
        {
            if (!_exitButton) _exitButton = GetComponent<Button>();
        }
    }
}