using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class Vibration : MonoBehaviour
{
    private Gamepad _gamepad;
    private PlayerPropSetting _playerProp;

    // Start is called before the first frame update
    void Start()
    {
        //ゲームパッドを取得
        var device = gameObject.GetComponent<PlayerInput>().devices;
        foreach (var d in device)
        {
            if (d is Gamepad x)
            {
                _gamepad = x;
                break;
            }
        }

        //敵情報取得
        _playerProp = gameObject.GetComponent<PlayerPropSetting>();

        StopMotor();
    }

    // Update is called once per frame
    void Update()
    {
        if(_gamepad is null)
            return;

        float vibration = 0.0f;
        Transform playerTransform = transform;

        foreach (var enemy in _playerProp.playerMana.Character)
        {
            if(enemy == gameObject || enemy is null)
                continue;

            Transform enemyTransform = enemy.transform;

            //敵との距離をからバイブレーションの強さを求める
            float length = 1.0f - (
                Mathf.Clamp(
                    Mathf.Abs((enemyTransform.position - playerTransform.position).magnitude), 0.0f, _playerProp.playerProperty.DistanceVibration) /
                _playerProp.playerProperty.DistanceVibration
            );

            vibration = Mathf.Max(_playerProp.playerProperty.MaxVibration * length, vibration);
        }

        _gamepad.SetMotorSpeeds(0.0f, vibration);
    }

    private void OnDisable()
    {
        StopMotor();
    }

    private void OnDestroy()
    {
        StopMotor();
    }

    void StopMotor()
    {
        if (!(_gamepad is null))
            _gamepad.SetMotorSpeeds(0.0f, 0.0f);
    }
}
