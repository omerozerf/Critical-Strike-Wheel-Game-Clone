using System;
using ScreenSystem;
using UnityEngine;
using UnityEngine.UI;

namespace ButtonSystem
{
    public class CollectRewardsButton : MonoBehaviour
    {
        [SerializeField] private Button _collectRewardsButton;
        [SerializeField] private ScreenController _screenController;


        private void OnValidate()
        {
            InitializeCollectRewardsButton();
        }

        
        private void InitializeCollectRewardsButton()
        {
            if (_collectRewardsButton == null) _collectRewardsButton = GetComponent<Button>();
        }
    }
}