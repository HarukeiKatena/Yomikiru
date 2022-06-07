using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;
using Player;
using Yomikiru.Effect;

namespace Enemy
{
    public class AIEnemyAttack : MonoBehaviour
    {
        [SerializeField] private Transform _swordGrip;
        [SerializeField] private Transform _swordRotator;
        [SerializeField] private Collider _swordCollider;
        [SerializeField] private ParticleSystem _swordTrail;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private GameObject _attackPrefub;

        [SerializeField] private EffectManager _effectManager;
        public EffectManager EffectManager {
            set { _effectManager = value; } 
        }

        [SerializeField] private int _interval;
        [SerializeField] private AudioClip _seSwing;
        [SerializeField] private float _swingSpeed;

        CancellationTokenSource _cts;

        void Start()
        {
            _cts = new CancellationTokenSource();
            _swordGrip.gameObject.SetActive(false);

        }

        void OnDisable()
        {
            _cts.Cancel();
        }

        async UniTaskVoid StopAttack(double time)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(time), cancellationToken: _cts.Token);
            _cts.Cancel();
        }

        async UniTaskVoid LoopAttack(CancellationToken token)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(8), cancellationToken: token);
            while(true){
                await AttackOnce();
                await UniTask.Delay(System.TimeSpan.FromSeconds(_interval), cancellationToken:token);
            }
        }
        
        public async UniTask AttackOnce()
        {
            _effectManager.Play("3D Hamon", transform.position);
            //剣を出す
            _swordGrip.gameObject.SetActive(true);
            var originalGripPos = _swordGrip.localPosition;
            _swordGrip.DOLocalMove(Vector3.zero, _swingSpeed);
            await _swordGrip.DOLocalRotate(new Vector3(0, 90, 70), _swingSpeed).AwaitForComplete(cancellationToken: _cts.Token);

            //剣を回す
            Instantiate(_attackPrefub, transform.position, Quaternion.identity);
            _swordCollider.enabled = true;
            _swordTrail.Play();
            _audioSource.PlayOneShot(_seSwing);
            await _swordRotator.DOLocalRotate(new Vector3(0, -360, 0), 0.5f, RotateMode.FastBeyond360).AwaitForStepComplete(cancellationToken: _cts.Token);

            //剣を戻す
            _swordTrail.Stop();
            _swordGrip.DOLocalMove(originalGripPos, _swingSpeed);
            await _swordGrip.DOLocalRotate(Vector3.zero, _swingSpeed).AwaitForComplete(cancellationToken: _cts.Token);

            //yield return null;

            //初期状態に戻す
            _swordRotator.localRotation = Quaternion.identity;
            _swordCollider.enabled = false;
            _swordGrip.gameObject.SetActive(false);
        }
    }
}
