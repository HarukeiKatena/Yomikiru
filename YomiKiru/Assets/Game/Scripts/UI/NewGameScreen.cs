using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;

namespace Yomikiru.UI
{

    // ゲーム作るシーンで表示する1画面のベース (シーンじゃないよ)
    [RequireComponent(typeof(CanvasGroup))]
    public class NewGameScreen : MonoBehaviour
    {

        [SerializeField] private CanvasGroup group;
        [SerializeField] private GameObject selectOnShow;
        [SerializeField] private Button backButton;

        private readonly Subject<Unit> onBack = new Subject<Unit>();

        public IObservable<Unit> OnBack => onBack;

        private void Reset()
        {
            group = GetComponent<CanvasGroup>();
        }

        private void Awake()
        {
            if(backButton != null)
            {
                backButton.onClick.AddListener(() => onBack.OnNext(Unit.Default));
            }
        }

        public async UniTask ShowAsync()
        {
            gameObject.SetActive(true);
            await DOTween.Sequence()
                .Join(group.DOFade(1, 0.2f).From(0).SetEase(Ease.OutCubic))
                .Join(transform.DOLocalMoveX(0, 0.2f).From(20).SetEase(Ease.OutCubic))
                .AsyncWaitForCompletion();
            group.interactable = true;
            EventSystem.current.SetSelectedGameObject(selectOnShow);
        }

        public void Show()
        {
            ShowAsync().Forget();
        }

        public async UniTask HideAsync()
        {
            EventSystem.current.SetSelectedGameObject(null);
            group.interactable = false;
            await DOTween.Sequence()
                .Join(group.DOFade(0, 0.2f).SetEase(Ease.OutCubic))
                .Join(transform.DOLocalMoveX(-20, 0.2f).SetEase(Ease.OutCubic))
                .AsyncWaitForCompletion();
            gameObject.SetActive(false);
        }

        public void Hide()
        {
            HideAsync().Forget();
        }

    }

}
