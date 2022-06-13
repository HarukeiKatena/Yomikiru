using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class CameraMove : MonoBehaviour
    {
        //色々
        private PlayerPropSetting _playerSetting;

        //定数
        private const float _threshold = 0.01f;

        //変数
        private float _cinemachineTargetPitch;
        private float _rotationVelocity;

        // Start is called before the first frame update
        void Start()
        {
            _playerSetting = gameObject.GetComponent<PlayerPropSetting>();
        }

        // Update is called once per frame
        void LateUpdate()
        {
            CameraRotation();
        }

        private void CameraRotation()
        {
            // if there is an input
            if (_playerSetting.inputRecord.look.sqrMagnitude >= _threshold)
            {
                float deltaTimeMultiplier = 1.0f;

                //Don't multiply mouse input by Time.deltaTime
                deltaTimeMultiplier = _playerSetting.EnableKeybodeMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetPitch += _playerSetting.inputRecord.look.y * _playerSetting.playerProperty.RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = _playerSetting.inputRecord.look.x * _playerSetting.playerProperty.RotationSpeed * deltaTimeMultiplier;

                // clamp our pitch rotation
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _playerSetting.playerProperty.BottomClamp, _playerSetting.playerProperty.TopClamp);

                // Update Cinemachine camera target pitch
                _playerSetting.CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

                // rotate the player left and right
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}
