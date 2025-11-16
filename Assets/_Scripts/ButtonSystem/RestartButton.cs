using Cysharp.Threading.Tasks;
using DG.Tweening;
using ScreenSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WheelSystem;
using ZoneSystem;

namespace ButtonSystem
{
    public class RestartButton : MonoBehaviour
    {
        [SerializeField] private Button _restartButton;


        private void Awake()
        {
            _restartButton.onClick.AddListener(HandleRestartButtonClicked);
        }
        
        private void OnDestroy()
        {
            _restartButton.onClick.RemoveListener(HandleRestartButtonClicked);
        }

        private void OnValidate()
        {
            InitializeRestartButton();
        }
        
        
        private async void HandleRestartButtonClicked()
        {
            DOTween.KillAll();
            await UniTask.Yield();

            await SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
        

        private void InitializeRestartButton()
        {
            if (_restartButton == null) _restartButton = GetComponent<Button>();
        }
    }
}