using DG.Tweening;

namespace ScreenSystem
{
    public class WinScreenController : ScreenController
    {
        public override void Show()
        {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.DOFade(1f, 0.5f).SetEase(Ease.InOutSine);
        }

        public override void Hide()
        {
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.DOFade(0f, 0.5f).SetEase(Ease.InOutSine);
        }
    }
}