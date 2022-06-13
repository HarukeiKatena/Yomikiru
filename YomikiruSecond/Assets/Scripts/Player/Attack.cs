using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using DG.Tweening;
using UniRx;

namespace Player
{
    public class Attack : MonoBehaviour
    {
        //U
        public Transform SwordGrip;
        public Transform SwordRotator;
        public Collider SwordCollider;
        public ParticleSystem SwordTrail;
        public GameObject AttackPrefub;

        //Ýč
        private PlayerPropSetting _playerSetting;

        private bool _isAttack;

        private Subject<Unit> attack = new Subject<Unit>();
        public IObservable<Unit> _onAttack => attack;

        // Start is called before the first frame update
        void Start()
        {
            _playerSetting = this.gameObject.GetComponent<PlayerPropSetting>();

            _playerSetting.inputRecord.Attack.
                Where(x => x && !_isAttack).
                Subscribe(_ => {
                    StartCoroutine(DoAttack());
                }).AddTo(this);

            SwordGrip.gameObject.SetActive(false);
        }

        private void Update()
        {
        }

        private IEnumerator DoAttack()
        {
            //剣を出す
            SwordGrip.gameObject.SetActive(true);
            _isAttack = true;
            var originalGripPos = SwordGrip.localPosition;
            SwordGrip.DOLocalMove(Vector3.zero, _playerSetting.playerProperty.AttackPopOutSpeed).WaitForCompletion();
            yield return SwordGrip.DOLocalRotate(new Vector3(0, 90, 70), _playerSetting.playerProperty.AttackPopOutSpeed).WaitForCompletion();

            attack.OnNext(Unit.Default);
            //イベント発行
            _playerSetting.OnAttack.OnNext(Unit.Default);

            _playerSetting.effectNamager.Play("3D Hamon", transform.position);

            //剣を回す
            Instantiate(AttackPrefub, transform.position, Quaternion.identity);
            SwordCollider.enabled = true;
            SwordTrail.Play();
            _playerSetting.Sound.PlaySE(_playerSetting.playerProperty.seSwing.name, transform.position);
            yield return SwordRotator.DOLocalRotate(new Vector3(0, -360, 0), _playerSetting.playerProperty.AttaclSpeed, RotateMode.FastBeyond360).WaitForCompletion();

            //剣を戻す
            SwordTrail.Stop();
            SwordGrip.DOLocalMove(originalGripPos, _playerSetting.playerProperty.AttackPopOutSpeed).WaitForCompletion();
            yield return SwordGrip.DOLocalRotate(Vector3.zero, _playerSetting.playerProperty.AttackPopOutSpeed).WaitForCompletion();

            yield return null;

            //初期状態に戻す
            SwordRotator.localRotation = Quaternion.identity;
            SwordCollider.enabled = false;
            _isAttack = false;
            _playerSetting.inputRecord.attack = false;
            SwordGrip.gameObject.SetActive(false);
        }
    }

}