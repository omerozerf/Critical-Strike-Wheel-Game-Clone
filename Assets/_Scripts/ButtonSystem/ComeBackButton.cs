using System;
using ScreenSystem;
using UnityEngine;
using UnityEngine.UI;

namespace ButtonSystem
{
    public class ComeBackButton : MonoBehaviour
    {
        [SerializeField] private Button _comeBackButton;
        [SerializeField] private ScreenController _screenController;


        private void Awake()
        {
            _comeBackButton.onClick.AddListener(HandleComeBackButtonClicked);
        }

        private void OnDestroy()
        {
            _comeBackButton.onClick.RemoveListener(HandleComeBackButtonClicked);
        }

        private void OnValidate()
        {
            InitializeComeBackButton();
        }


        private void HandleComeBackButtonClicked()
        {
            _screenController.Hide();
        }
        
        
        private void InitializeComeBackButton()
        {
            if (_comeBackButton == null) _comeBackButton = GetComponent<Button>();
        }
    }
}