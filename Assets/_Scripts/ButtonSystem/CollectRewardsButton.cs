using System;
using ScreenSystem;
using UnityEngine;
using UnityEngine.UI;

namespace ButtonSystem
{
    public class CollectRewardsButton : MonoBehaviour
    {
        [SerializeField] private Button _collectRewardsButton;
        [SerializeField] private ScreenController _winScreenController;
        [SerializeField] private ScreenController _rewardsScreenController;

        public static event Action OnClicked;
        

        private void Awake()
        {
            _collectRewardsButton.onClick.AddListener(HandleOnClicked);
        }

        private void OnDestroy()
        {
            _collectRewardsButton.onClick.RemoveListener(HandleOnClicked);
        }

        private void OnValidate()
        {
            InitializeCollectRewardsButton();
        }

        
        private void HandleOnClicked()
        {
            _winScreenController.Hide();
            _rewardsScreenController.Show();
            
            OnClicked?.Invoke();
        }
        
        
        private void InitializeCollectRewardsButton()
        {
            if (_collectRewardsButton == null) _collectRewardsButton = GetComponent<Button>();
        }
    }
}