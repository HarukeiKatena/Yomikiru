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

        // 武器
        [SerializeField] private Transform swordGrip;
        [SerializeField] private Transform swordRotator;
        [SerializeField] private Collider swordCollider;
        [SerializeField] private ParticleSystem swordTrail;
        [SerializeField] private GameObject attackPrefab;

        // 状態
        private bool isAttack;

        private void Awake()
        {
            TryGetComponent(out character);
        }

        private void Start()
        {
            table = character.Table;

            swordGrip.gameObject.SetActive(false);
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
            swordGrip.gameObject.SetActive(true);
            isAttack = true;
            var originalGripPos = swordGrip.localPosition;
            swordGrip.DOLocalMove(Vector3.zero, table.AttackPopOutSpeed).WaitForCompletion();
            yield return swordGrip.DOLocalRotate(new Vector3(0, -90, -70), table.AttackPopOutSpeed).WaitForCompletion();

            //イベント発行

            //剣を回す
            Instantiate(attackPrefab, transform.position, Quaternion.identity);
            swordCollider.enabled = true;
            swordTrail.Play();
            // play sound
            //yield return SwordRotator.DOLocalRotate(new Vector3(0, -360, 0), table.AttaclSpeed, RotateMode.FastBeyond360).WaitForCompletion();
            yield return DoRotateAround(swordGrip, swordRotator, -360, table.AttackSpeed).WaitForCompletion();
            //剣を戻す
            swordTrail.Stop();
            swordGrip.DOLocalMove(originalGripPos, table.AttackPopOutSpeed).WaitForCompletion();
            yield return swordGrip.DOLocalRotate(Vector3.zero, table.AttackPopOutSpeed).WaitForCompletion();

            yield return null;

            //初期状態に戻す
            swordRotator.localRotation = Quaternion.identity;
            swordCollider.enabled = false;
            isAttack = false;
            swordGrip.gameObject.SetActive(false);
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
