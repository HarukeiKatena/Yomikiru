using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Player
{
    public class Jump : MonoBehaviour
    {
        [SerializeField] private bool Grounded = true;

        //�F�X
        private PlayerPropSetting _playerSetting;
        private CharacterController _controller;

        //�W�����v
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        public bool IsJump = false;
        private bool IsLanding = true;

        //�ړ�
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
                // �����N�[���^�C�����Z�b�g
                _fallTimeoutDelta = _playerSetting.playerProperty.FallTimeout;

                // �㉺�̉����x���������Ȑ��l�ɂȂ�Ȃ��悤�ɗ}������
                if (verticalVelocity < -2f)
                {
                    verticalVelocity = -2f;
                }

                //�㏸���ɓV��ɓ��������ꍇ������������
                if (_controller.velocity.y == 0.0f)
                {
                    verticalVelocity = 0.0f;
                }

                // �W�����v�N�[���^�C��
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }

                //���n
                if (!IsLanding && !IsJump)
                {
                    _playerSetting.effectNamager.Play("2D Hamon", transform.position);
                    IsLanding = true;
                }

                IsJump = false;
            }
            else
            {
                // �W�����v�N�[���^�C�����Z�b�g
                _jumpTimeoutDelta = _playerSetting.playerProperty.JumpTimeout;

                // �����N�[���^�C��
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }

                //�㏸���ɓV��ɓ��������ꍇ������������
                if (_controller.velocity.y == 0.0f)
                {
                    verticalVelocity = 0.0f;
                }

                //�W�����v���͎󂯕t����悤�ɂ���
                _playerSetting.inputRecord.jump = false;

                IsJump = true;
                IsLanding = false;
            }

            // �d�͂ŗ��Ƃ�
            if (verticalVelocity < _terminalVelocity)
            {
                verticalVelocity += (_playerSetting.playerProperty.Gravity * Time.deltaTime);
            }
        }

        void JumpStep()
        {
            // H * -2 * G �̕����� = �]�݂̍����ɓ��B���邽�߂ɕK�v�ȑ��x
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
