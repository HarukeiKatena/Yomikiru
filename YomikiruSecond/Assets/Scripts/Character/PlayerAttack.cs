using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using Player;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Yomikiru.Input;

namespace Yomikiru.Character
{
    [RequireComponent(typeof(Character))]
    public class PlayerAttack : MonoBehaviour
    {
        // CancellationTokenSourceを生成
        private CancellationTokenSource cts;
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
            cts = new CancellationTokenSource();
            TryGetComponent(out character);
        }

        private void Start()
        {
            table = character.Table;

            swordGrip.gameObject.SetActive(false);
        }

        public void OnAttack()
        {
            if(isAttack) return;
            table.Effect.Request(this.name, table.AttackEffect, transform.position);
            AttackAsync(cts.Token).Forget();
        }

        private async UniTask AttackAsync(CancellationToken token)
        {
            //Debug.Log("Attack");
            //剣を出す
            swordGrip.gameObject.SetActive(true);
            isAttack = true;
            var originalGripPos = swordGrip.localPosition;
            await UniTask.WhenAll(
                swordGrip.DOLocalMove(Vector3.zero, table.AttackPopOutSpeed).ToUniTask(cancellationToken: token),
                swordGrip.DOLocalRotate(new Vector3(0, -90, -70), table.AttackPopOutSpeed).ToUniTask(cancellationToken: token)
            );

            //イベント発行

            //剣を回す
            Instantiate(attackPrefab, transform.position, Quaternion.identity);
            swordCollider.enabled = true;
            swordTrail.Play();
            // play sound
            await swordRotator.DOLocalRotate(new Vector3(0, -360, 0), table.AttackSpeed, RotateMode.FastBeyond360).ToUniTask(cancellationToken: token);
            //剣を戻す
            swordTrail.Stop();
            await UniTask.WhenAll(
                swordGrip.DOLocalMove(originalGripPos, table.AttackPopOutSpeed).ToUniTask(cancellationToken: token),
                swordGrip.DOLocalRotate(Vector3.zero, table.AttackPopOutSpeed).ToUniTask(cancellationToken: token)
            );

            await UniTask.Yield(cancellationToken: token);

            //初期状態に戻す
            swordRotator.localRotation = Quaternion.identity;
            swordCollider.enabled = false;
            isAttack = false;
            swordGrip.gameObject.SetActive(false);
        }
    }
}
