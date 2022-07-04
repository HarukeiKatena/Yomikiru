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
    [RequireComponent(typeof(InputEvent))]
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
            AttackAsync(cts.Token).Forget();
        }

        private async UniTask AttackAsync(CancellationToken cts)
        {
            Debug.Log("Attack");
            //剣を出す
            swordGrip.gameObject.SetActive(true);
            isAttack = true;
            var originalGripPos = swordGrip.localPosition;
            swordGrip.DOLocalMove(Vector3.zero, table.AttackPopOutSpeed).WaitForCompletion();
            await swordGrip.DOLocalRotate(new Vector3(0, -90, -70), table.AttackPopOutSpeed);

            //イベント発行

            //剣を回す
            Instantiate(attackPrefab, transform.position, Quaternion.identity);
            swordCollider.enabled = true;
            swordTrail.Play();
            // play sound
            await swordRotator.DOLocalRotate(new Vector3(0, -360, 0), table.AttackSpeed, RotateMode.FastBeyond360);
            //await DoRotateAround(swordGrip, swordRotator, -360, table.AttackSpeed);
            //剣を戻す
            swordTrail.Stop();
            swordGrip.DOLocalMove(originalGripPos, table.AttackPopOutSpeed).WaitForCompletion();
            await swordGrip.DOLocalRotate(Vector3.zero, table.AttackPopOutSpeed);

            await UniTask.Yield();

            //初期状態に戻す
            swordRotator.localRotation = Quaternion.identity;
            swordCollider.enabled = false;
            isAttack = false;
            swordGrip.gameObject.SetActive(false);
        }
    }
}
