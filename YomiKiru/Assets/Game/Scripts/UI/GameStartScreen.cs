using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;

namespace Yomikiru.UI
{

    public class GameStartScreen : NewGameScreen
    {

        [SerializeField] private MapButton mapButton;
        [SerializeField] private Button buttonReady;
        [SerializeField] private GameObject playersContainer;
        [SerializeField] private PlayerJoin playerPrefab;
        [SerializeField] private CheckController controllerCheck;

        private readonly List<PlayerJoin> players = new List<PlayerJoin>();

        public Button.ButtonClickedEvent OnReady => buttonReady.onClick;

        private void Start()
        {
            controllerCheck.ChangePlayerDevice.Subscribe(data =>
            {
                DisplayPlayers();
            }).AddTo(this);
        }

        public void Display(GamemodeInfo gamemode, MapInfo map, CharacterInfo character)
        {
            mapButton.Map = map;
            DisplayPlayers();
        }

        private void DisplayPlayers()
        {
            var playerCount = ControllerManagement.PlayerCount;
            // プレイヤーリストに追加
            while (players.Count < playerCount)
            {
                var player = Instantiate(playerPrefab, playersContainer.transform);
                players.Add(player);
                player.PlayerName = $"{players.Count.ToString()}P";
            }
            // 削除 (2人モード選んで戻って1人モード選んだ時の2個目 etc)
            if(players.Count > playerCount)
            {
                for (int i = playerCount; i < players.Count; i++)
                {
                    Destroy(players[i].gameObject);
                }
                players.RemoveRange(playerCount, players.Count - playerCount);
            }
            for (int i = 0; i < playerCount; i++)
            {
                var gamepad = ControllerManagement.PlayerDevice[i];
                if (gamepad == null)
                {
                    if (i == ControllerManagement.KeybordPlayerIndex)
                    {
                        // キーボード (deviceにnullじゃなくてKeyboard渡したい)
                        players[i].DisplayJoined(gamepad, true);
                    }
                    else
                    {
                        // 無登録
                        players[i].DisplayAwaitingInput(ControllerManagement.KeybordPlayerIndex == -1);
                    }
                }
                else
                {
                    // ゲームパッド
                    players[i].DisplayJoined(gamepad, false);
                }
            }
            bool allControllersSet = ControllerManagement.IsControllersForAllPlayersSet();
            buttonReady.interactable = allControllersSet;
            // 退出で準備完了ボタンが選択解除されるので再参加で再選択
            if (allControllersSet && EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(buttonReady.gameObject);
            }
        }

    }

}
