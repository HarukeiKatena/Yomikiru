using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
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
            CharacterList = new GameObject[ControllerManager.MaxPlayerCount];

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
                    var pads = Gamepad.all;
                    if (pads.Count >= index + 1)//ゲームパッドがあればパッドにする
                    {
                        SetDeviceAndScheme(user, pads[index], "Gamepad");
                        controllerManager.PlayerDevices[index] = pads[index];
                    }
                    else if (controllerManager.KeybordPlayerIndex == ControllerManager.NotUsedKeybord)
                    {
                        SetDevicesAndScheme(user, new InputDevice[] {Keyboard.current, Mouse.current}, "KeyboardMouse");
                        controllerManager.KeybordPlayerIndex = index;
                    }
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
            var cameraparent = FindCameraParent(parentObject.transform);
            if(cameraparent == null)
                return;

            int layerindex = LayerMask.NameToLayer("P" + (playerIndex + 1));

            //カメラの設定
            var cameraobject = cameraparent.Find("Camera");
            if(cameraobject != null && cameraobject.TryGetComponent(out Camera camera))
            {
                camera.cullingMask ^= 1 << layerindex;
                camera.depth = ControllerManager.MaxPlayerCount - playerIndex;
                camera.rect = controllerManager.PlayerCount switch
                {
                    1 => new Rect(0.0f, 0.0f, 1.0f, 1.0f),
                    2 => playerIndex == 0 ? new Rect(0.0f, 0.5f, 1.0f, 0.5f) : new Rect(0.0f, 0.0f, 1.0f, 0.5f),
                    _ => camera.rect
                };
            }

            //CinemachineVirtualCameraの設定
            var vcobject = cameraparent.Find("VirtualCamera");
            if(vcobject != null && vcobject.TryGetComponent(out CinemachineVirtualCamera vc))
            {
                vc.gameObject.layer = layerindex;
            }
        }

        private Transform FindCameraParent(Transform parent)
        {
            foreach (Transform child in parent.transform)
            {
                if (child.gameObject.name == "Camera")
                    return child.parent;

                var parentobj = FindCameraParent(child);
                if (parentobj != null)
                    return parentobj;
            }
            return null;
        }

        public Camera GetCharacterCamera(int playerIndex)
        {
            return FindCameraParent(CharacterList[playerIndex].transform).
                transform.Find("Camera").GetComponent<Camera>();
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
