using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using Yomikiru.Controller;

namespace Yomikiru.Characte.Management
{
    [RequireComponent(typeof(PlayerInputManager))]

    public class CharacterManagement : MonoBehaviour
    {
        [SerializeField] private ControllerManager controllerManager;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private Transform[] startPosition;

        [HideInInspector] public GameObject[] CharacterList;

        private void Awake()
        {
            //個数集める
            CharacterList = new GameObject[controllerManager.PlayerDevices.Length];

            //プレイヤー
            for (int i = 0; i < controllerManager.PlayerCount; i++)
                CreatePlayer(i);

            //プレイヤー数が1の場合エネミー作成する
            if (controllerManager.PlayerCount == 1)
                CreateEnemy(1);
        }

        private void CreatePlayer(int index)
        {
            var player = PlayerInput.Instantiate(playerPrefab);
            player.transform.SetPositionAndRotation(
                startPosition[index].position,
                startPosition[index].rotation);

            //事前に指定したデバイスをセットする
            var user = player.user;
            user.UnpairDevices();
            if (controllerManager.KeybordPlayerIndex == index)
                SetDevicesAndScheme(user, new InputDevice[] {Keyboard.current, Mouse.current}, "KeyboardMouse");
            else
                SetDeviceAndScheme(user, controllerManager.PlayerDevices[index], "Gamepad");

            //キャラクター保持
            CharacterList[index] = player.gameObject;
        }

        private void CreateEnemy(int index)
        {
            var enemy = GameObject.Instantiate(
                enemyPrefab,
                startPosition[index].position,
                startPosition[index].rotation);

            //キャラクター保持
            CharacterList[index] = enemy.gameObject;
        }

        private void SetDevicesAndScheme(InputUser user, InputDevice[] devices, string scheme)
        {
            //デバイスを一個ずつセットする(まとめて入れる方法がないため)
            foreach (var device in devices)
                InputUser.PerformPairingWithDevice(device, user);
            user.ActivateControlScheme(scheme);
        }

        private void SetDeviceAndScheme(InputUser user, InputDevice device, string scheme)
        {
            InputUser.PerformPairingWithDevice(device, user);
            user.ActivateControlScheme(scheme);
        }

        //引数でしたいしたオブジェクトを除いて一番最初にヒットしたオブジェクトを返す
        public GameObject GetCharacterObject(GameObject ExcludedObject)
        {
            return CharacterList.FirstOrDefault(character => ExcludedObject != character);
        }

        //引数でしたいしたオブジェクトを除いてヒットしたオブジェクトのリストを返す
        public List<GameObject> GetCharacterObjectList(GameObject ExcludedObject)
        {
            return CharacterList.Where(character => character != ExcludedObject).ToList();
        }
    }
}
