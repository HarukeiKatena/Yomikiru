using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Yomikiru.Effect;

namespace Player
{
    public class Move : MonoBehaviour
    {
        //色々
        private PlayerPropSetting _playerSetting;
        private CharacterController _controller;
        private Jump _jump;
        private int _walkSEIndex = 0;

        //数値
        private float _speed;

        // Start is called before the first frame update
        void Start()
        {
            _playerSetting = gameObject.GetComponent<PlayerPropSetting>();
            _controller = gameObject.GetComponent<CharacterController>();
            _jump = gameObject.GetComponent<Jump>();
        }

        // Update is called once per frame
        void Update()
        {
            MoveStep();
        }

        void MoveStep()
        {
            if(!_controller.enabled)
                return;

            Vector2 move = _playerSetting.inputRecord.Move.Value;

            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _playerSetting.inputRecord.sprint ? _playerSetting.playerProperty.SprintSpeed : _playerSetting.playerProperty.MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = move.magnitude;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * _playerSetting.playerProperty.SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            // normalise input direction
            Vector3 inputDirection = new Vector3(move.x, 0.0f, move.y).normalized;
            var horizontalVelocity = inputDirection.normalized * _speed;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (move != Vector2.zero)
            {
                // move
                inputDirection = transform.right * move.x + transform.forward * move.y;

                var audioSource = _playerSetting.audioSource;
                if (!audioSource.isPlaying && !_jump.IsJump)
                {
                    if ((horizontalVelocity.magnitude / _playerSetting.playerProperty.MoveSpeed) > 0.5f)
                    {
                        _playerSetting.effectNamager.Play("Asioto", transform.position);
                    }
                    audioSource.PlayOneShot(_playerSetting.playerProperty.seWalk[_walkSEIndex % _playerSetting.playerProperty.seWalk.Length]);
                }
            }

            // move the player
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0f, _jump.verticalVelocity, 0f) * Time.deltaTime);
        }
    }
}
