using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;
using Player;
using Yomikiru.Effect;

namespace Yomikiru.Enemy
{
    public class AIEnemyAttack : MonoBehaviour
    {
        [SerializeField] private Transform swordGrip;
        [SerializeField] private Transform swordRotator;
        [SerializeField] private Collider swordCollider;
        [SerializeField] private ParticleSystem swordTrail;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private GameObject attackPrefab;

        [SerializeField] private EffectManager effectManager;
        public EffectManager EffectManager
        {
            set { effectManager = value; }
        }

        [SerializeField] private int interval;
        [SerializeField] private AudioClip seSwing;
        [SerializeField] private float swingSpeed;

        private CancellationTokenSource cts;

        void Start()
        {
            cts = new CancellationTokenSource();
            swordGrip.gameObject.SetActive(false);

        }

        void OnDisable()
        {
            cts.Cancel();
        }

        async UniTaskVoid StopAttack(double time)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(time), cancellationToken: cts.Token);
            cts.Cancel();
        }

        async UniTaskVoid LoopAttack(CancellationToken token)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(8), cancellationToken: token);
            while (true)
            {
                await AttackOnce();
                await UniTask.Delay(System.TimeSpan.FromSeconds(interval), cancellationToken: token);
            }
        }

        public async UniTask AttackOnce()
        {
            effectManager.Play("3D Hamon", transform.position);
            //剣を出す
            swordGrip.gameObject.SetActive(true);
            var originalGripPos = swordGrip.localPosition;
            swordGrip.DOLocalMove(Vector3.zero, swingSpeed);
            await swordGrip.DOLocalRotate(new Vector3(0, 90, 70), swingSpeed).AwaitForComplete(cancellationToken: cts.Token);

            //剣を回す
            Instantiate(attackPrefab, transform.position, Quaternion.identity);
            swordCollider.enabled = true;
            swordTrail.Play();
            audioSource.PlayOneShot(seSwing);
            await swordRotator.DOLocalRotate(new Vector3(0, -360, 0), 0.5f, RotateMode.FastBeyond360).AwaitForStepComplete(cancellationToken: cts.Token);

            //剣を戻す
            swordTrail.Stop();
            swordGrip.DOLocalMove(originalGripPos, swingSpeed);
            await swordGrip.DOLocalRotate(Vector3.zero, swingSpeed).AwaitForComplete(cancellationToken: cts.Token);

            //yield return null;

            //初期状態に戻す
            swordRotator.localRotation = Quaternion.identity;
            swordCollider.enabled = false;
            swordGrip.gameObject.SetActive(false);
        }
    }
}
