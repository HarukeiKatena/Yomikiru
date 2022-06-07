using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Yomikiru.UI
{

    public class NewGameScene : MonoBehaviour
    {

        [SerializeField] private GameScriptableObject matchInfo;
        [SerializeField] private GamemodeSelectScreen gamemodeSelectScreen;
        [SerializeField] private MapSelectScreen mapSelectScreen;
        [SerializeField] private CharacterSelectScreen characterSelectScreen;
        [SerializeField] private GameStartScreen gameStartScreen;

        private GamemodeInfo gamemode;
        private MapInfo map;
        private CharacterInfo character;
        private NewGameScreen currentScreen;

        private void Start()
        {
            gamemodeSelectScreen.OnGamemodeSelect.Subscribe(gamemode =>
            {
                this.gamemode = gamemode;
                // gamemode か gamemode.Gamemode に人数持たせるべきな気がする
                ControllerManagement.PlayerCount = gamemode.Gamemode switch
                {
                    Gamemode.DUELS => 2,
                    Gamemode.SOLO  => 1,
                    _ => throw new ArgumentOutOfRangeException(nameof(gamemode.Gamemode)),
                };
                ShowMapSelect();
            }).AddTo(this);
            mapSelectScreen.OnMapSelect.Subscribe(map =>
            {
                this.map = map;
                ShowGameStart();
            }).AddTo(this);
            gameStartScreen.OnReady.AddListener(() => {
                matchInfo.Gamemode = gamemode;
                matchInfo.Map = map;
                GameManager.Instance.LoadScene(map.SceneName);
            });

            gamemodeSelectScreen.OnBack.Subscribe(_ => GameManager.Instance.LoadScene("Title")).AddTo(this);
            mapSelectScreen.OnBack.Subscribe(_ => ShowGamemodeSelect()).AddTo(this);
            gameStartScreen.OnBack.Subscribe(_ => ShowMapSelect()).AddTo(this);

            currentScreen = gamemodeSelectScreen;
            gamemodeSelectScreen.ShowAsync().Forget();
        }

        private void ShowMapSelect() => ShowScreenAsync(mapSelectScreen).Forget();

        private void ShowGamemodeSelect() => ShowScreenAsync(gamemodeSelectScreen).Forget();

        private void ShowGameStart()
        {
            gameStartScreen.Display(gamemode, map, character);
            ShowScreenAsync(gameStartScreen).Forget();
        }

        private async UniTask ShowScreenAsync(NewGameScreen screen)
        {
            await currentScreen.HideAsync();
            currentScreen = screen;
            await currentScreen.ShowAsync();
        }

    }

}
