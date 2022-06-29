using System.Collections;
using System.Collections.Generic;
using System;
using Player;
using UnityEngine;
using UniRx;
using DG.Tweening;
using Yomikiru.Input;

namespace Yomikiru.Character
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(InputEvent))]
    public class PlayerAttack : MonoBehaviour
    {
        // 内部コンポーネント
        private Character character;
        private CharacterData table;

        //武器
        public Transform SwordGrip;
        public Transform SwordRotator;
        public Collider SwordCollider;
        public ParticleSystem SwordTrail;
        public GameObject AttackPrefub;

        // 状態
        private bool isAttack;

        private void Awake()
        {
            TryGetComponent(out character);
        }

        private void Start()
        {
            table = character.Table;

            SwordGrip.gameObject.SetActive(false);
        }

        public void OnAttack()
        {
            if(!isAttack) 
            {
                StartCoroutine(DoAttack());
            }
        }

        private IEnumerator DoAttack()
        {
            Debug.Log("Attack");
            //剣を出す
            SwordGrip.gameObject.SetActive(true);
            isAttack = true;
            var originalGripPos = SwordGrip.localPosition;
            SwordGrip.DOLocalMove(Vector3.zero, table.AttackPopOutSpeed).WaitForCompletion();
            yield return SwordGrip.DOLocalRotate(new Vector3(0, -90, -70), table.AttackPopOutSpeed).WaitForCompletion();

            //イベント発行

            //剣を回す
            Instantiate(AttackPrefub, transform.position, Quaternion.identity);
            SwordCollider.enabled = true;
            SwordTrail.Play();
            // play sound
            //yield return SwordRotator.DOLocalRotate(new Vector3(0, -360, 0), table.AttaclSpeed, RotateMode.FastBeyond360).WaitForCompletion();
            yield return DoRotateAround(SwordGrip, SwordRotator, -360, table.AttaclSpeed).WaitForCompletion();
            //剣を戻す
            SwordTrail.Stop();
            SwordGrip.DOLocalMove(originalGripPos, table.AttackPopOutSpeed).WaitForCompletion();
            yield return SwordGrip.DOLocalRotate(Vector3.zero, table.AttackPopOutSpeed).WaitForCompletion();

            yield return null;

            //初期状態に戻す
            SwordRotator.localRotation = Quaternion.identity;
            SwordCollider.enabled = false;
            isAttack = false;
            SwordGrip.gameObject.SetActive(false);
        }

        private float prevAngle = 0.0f;
        private Tween DoRotateAround(Transform target, Transform rotateAxis, float endValue, float duration)
        {
            prevAngle = 0.0f;

            // durationの時間で値を0～endValueまで変更させて公転処理を呼ぶ
            Tween ret = DOTween.To(x => RotateAroundPrc(target, rotateAxis, x), 0.0f, endValue, duration);
            return ret;
        }

        private void RotateAroundPrc(Transform target, Transform rotateAxis, float value)
        {
            float delta = value - prevAngle;

            target.RotateAround(rotateAxis.position, rotateAxis.eulerAngles, delta);

            prevAngle = value;
        }
    }
}
