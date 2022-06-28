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

        private void Start()
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
            {
                SetDevicesAndScheme(user, new InputDevice[] {Keyboard.current, Mouse.current}, "KeyboardMouse");
            }
            else
            {
                //パッドをセットする
                if (controllerManager.PlayerDevices[index] != null) {
                    SetDeviceAndScheme(user, controllerManager.PlayerDevices[index], "Gamepad");
                }
                else {//指定デバイスが無い場合
                    if (Gamepad.all.Count >= index + 1)//ゲームパッドがあればパッドにする
                        SetDeviceAndScheme(user, Gamepad.all[index], "Gamepad");
                    else if(index == ControllerManager.MaxPlayerCount - 1)
                        SetDevicesAndScheme(user, new InputDevice[] {Keyboard.current, Mouse.current}, "KeyboardMouse");
                }
            }

            //キャラクター保持
            var obj = player.gameObject;
            CharacterList[index] = obj;

            //カメラ
            CameraSetting(obj, index);
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

        private void CameraSetting(GameObject parentObject, int playerIndex)
        {
            var camera = CameraChildFind(parentObject.transform);
            if(camera == null)
                return;

            camera.rect = controllerManager.PlayerCount switch
            {
                1 => new Rect(0.0f, 0.0f, 1.0f, 1.0f),
                2 => playerIndex == 0 ? new Rect(0.0f, 0.5f, 1.0f, 0.5f) : new Rect(0.0f, 0.0f, 1.0f, 0.5f),
                _ => camera.rect
            };
        }

        private Camera CameraChildFind(Transform parent)
        {
            foreach (Transform child in parent.transform)
            {
                if (child.gameObject.name == "Camera")
                    return child.GetComponent<Camera>();

                var camera = CameraChildFind(child);
                if (camera != null)
                    return camera;
            }

            return null;
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
