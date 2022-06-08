using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Player
{
    public class Jump : MonoBehaviour
    {
        [SerializeField] private bool Grounded = true;

        //色々
        private PlayerPropSetting _playerSetting;
        private CharacterController _controller;

        //ジャンプ
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        public bool IsJump = false;
        private bool IsLanding = true;

        //移動
        [HideInInspector]
        public float verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // Start is called before the first frame update
        void Start()
        {
            _playerSetting = this.gameObject.GetComponent<PlayerPropSetting>();
            _controller = this.gameObject.GetComponent<CharacterController>();

            _playerSetting.inputRecord.Jump.
                Where(x => x && Grounded).
                Subscribe(_ => JumpStep()).AddTo(this);
        }

        // Update is called once per frame
        void Update()
        {
            GroundCheck();
            JumpAndGravity();
        }

        private void GroundCheck()
        {
            // set sphere position, with offset
            Grounded = HitCheck();
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // 落下クールタイムリセット
                _fallTimeoutDelta = _playerSetting.playerProperty.FallTimeout;

                // 上下の加速度がおかしな数値にならないように抑制する
                if (verticalVelocity < -2f)
                {
                    verticalVelocity = -2f;
                }

                //上昇中に天井に当たった場合即時落下する
                if (_controller.velocity.y == 0.0f)
                {
                    verticalVelocity = 0.0f;
                }

                // ジャンプクールタイム
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }

                //着地
                if (!IsLanding && !IsJump)
                {
                    _playerSetting.effectNamager.Play("2D Hamon", transform.position);
                    IsLanding = true;
                }

                IsJump = false;
            }
            else
            {
                // ジャンプクールタイムリセット
                _jumpTimeoutDelta = _playerSetting.playerProperty.JumpTimeout;

                // 落下クールタイム
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }

                //上昇中に天井に当たった場合即時落下する
                if (_controller.velocity.y == 0.0f)
                {
                    verticalVelocity = 0.0f;
                }

                //ジャンプ入力受け付けるようにする
                _playerSetting.inputRecord.jump = false;

                IsJump = true;
                IsLanding = false;
            }

            // 重力で落とす
            if (verticalVelocity < _terminalVelocity)
            {
                verticalVelocity += (_playerSetting.playerProperty.Gravity * Time.deltaTime);
            }
        }

        void JumpStep()
        {
            // H * -2 * G の平方根 = 望みの高さに到達するために必要な速度
            verticalVelocity = CalcHeightSpeed(_playerSetting.playerProperty.JumpHeight, _playerSetting.playerProperty.Gravity);
            Grounded = false;
            IsJump = true;
            IsLanding = false;
            //_playerSetting.effectNamager.Play("2D Hamon", transform.position);
        }

        float CalcHeightSpeed(float jumpHeight, float Gravity)
        {
            return Mathf.Sqrt(
                jumpHeight * -2f * Gravity);
        }

        bool HitCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _playerSetting.playerProperty.GroundedOffset, transform.position.z);
            return Physics.CheckSphere(spherePosition, _playerSetting.playerProperty.GroundedRadius, _playerSetting.playerProperty.GroundLayers, QueryTriggerInteraction.Ignore);
        }

        void OnDisable()
        {
            verticalVelocity = -2f;
        }
    }
}
