using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;

namespace Yomikiru.UI
{

    public class IntroDisplay : MonoBehaviour
    {

        [SerializeField] private CanvasGroup modeContainer;
        [SerializeField] private TextMeshProUGUI modeNameText;
        [SerializeField] private TextMeshProUGUI modeDescText;
        [SerializeField] private RectTransform modeBarLeft;
        [SerializeField] private RectTransform modeBarRight;
        [SerializeField] private RectTransform readyContainer;
        [SerializeField] private TextMeshProUGUI readyText;
        [SerializeField] private TextMeshProUGUI go;

        private Vector2 readyContainerSize;

        private void Start()
        {
            readyContainerSize = readyContainer.sizeDelta;
        }

        public async UniTask DisplayModeAsync(GamemodeInfo gamemode)
        {
            if(gamemode != null)
            {
                modeNameText.text = gamemode.Name;
                modeDescText.text = gamemode.Description;
            }
            modeContainer.gameObject.SetActive(true);
            readyContainer.gameObject.SetActive(false);
            await UniTask.WhenAll(
                modeContainer.DOFade(1, 0.3f).From(0).ToUniTask(),
                ShowBarsAsync()
            );
        }

        public async UniTask DisplayReadyAsync()
        {
            readyContainer.gameObject.SetActive(true);
            await DOTween.Sequence()
                .Join(readyContainer.DOSizeDelta(readyContainerSize, 1).From(new Vector2(readyContainerSize.x, 0)).SetEase(Ease.OutQuart))
                .Join(DOVirtual.Float(0, 75, 1, p => readyText.characterSpacing = p).SetEase(Ease.OutQuart));
        }

        public async UniTask DisplayGoAsync()
        {
            modeContainer.gameObject.SetActive(false);
            readyContainer.gameObject.SetActive(false);
            go.gameObject.SetActive(true);
            HideBarsAsync().Forget();
            _ = go.gameObject.transform.DOScale(1, 1).From(1.3f).SetEase(Ease.OutQuart);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5));
            await DOVirtual.Float(1, 0, 0.2f, p => go.alpha = Mathf.Round(p)).SetEase(Ease.Flash, 10);
            await go.DOFade(0, 0.2f);
        }

        private async UniTask ShowBarsAsync()
        {
            modeBarLeft.gameObject.SetActive(true);
            modeBarRight.gameObject.SetActive(true);
            modeBarLeft.pivot = new Vector2(1, 0.5f);
            modeBarRight.pivot = new Vector2(0, 0.5f);
            await DOTween.Sequence()
                .Join(modeBarLeft.DOScaleX(1, 2).From(0.5f).SetEase(Ease.OutQuart))
                .Join(modeBarRight.DOScaleX(1, 2).From(0.5f).SetEase(Ease.OutQuart));
        }

        private async UniTask HideBarsAsync()
        {
            modeBarLeft.pivot = new Vector2(0, 0.5f);
            modeBarRight.pivot = new Vector2(1, 0.5f);
            await DOTween.Sequence()
                .Join(modeBarLeft.DOScaleX(0, 0.5f).From(0.5f).SetEase(Ease.OutCubic))
                .Join(modeBarRight.DOScaleX(0, 0.5f).From(0.5f).SetEase(Ease.OutCubic));

            modeBarLeft.gameObject.SetActive(false);
            modeBarRight.gameObject.SetActive(false);
        }

    }

}
