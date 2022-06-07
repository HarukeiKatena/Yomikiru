using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
using Cysharp.Threading.Tasks;
//using System.Threading.Tasks;
using System.Threading;
using Yomikiru.Effect;

namespace Enemy{
    public class AIEnemyMove : MonoBehaviour
    {
        [SerializeField] private PlayerManagement _playerManager;
        public PlayerManagement PlayerManager{
            set { _playerManager = value; } 
        }

        [SerializeField] private EffectManager _effectManager;
        public EffectManager EffectManager { 
            set { _effectManager = value; } 
        }

        [SerializeField] private Player.Attack _playerAttack;
        [SerializeField] private Transform _playerTransform;
    
        [SerializeField] private NavMeshAgent _navMeshAgent;

        [SerializeField] private AreaBox _areaBox;

        [SerializeField] private float _hamonInterval;

        [SerializeField] private float _aroundTime;
        [SerializeField] private float _sightAngle;
        [SerializeField] private float _maxDistance;

        [SerializeField] private Yomikiru.MapInfo _mapInfo;

        private AIEnemyBase _enemyBase;
        private AIEnemyAttack _enemyAttack;

        private Vector3 _areaBoxPos;
        private bool _searching;

        [SerializeField] private bool _playerAttackFlag;

        private float _anglePerSecond;

        CancellationTokenSource _cts;
    
        // Start is called before the first frame update
        void Start()
        {
            _areaBox = _mapInfo.AreaBox;

            _cts = new CancellationTokenSource();

            _enemyBase = GetComponent<AIEnemyBase>();
            _enemyAttack = GetComponent<AIEnemyAttack>();

            var player =  _playerManager.Character[0];
            _playerTransform = player.GetComponent<Transform>();
            _playerAttack = player.GetComponent<Player.Attack>();
            _areaBoxPos = _areaBox.GetComponent<Transform>().position;
            
            _anglePerSecond = 360 / _aroundTime;
            _searching = false;
            _enemyBase._onStartGame.Subscribe(_ => LoopHamon(_cts.Token).Forget());
            _enemyBase._onStartGame.Subscribe(_ => SetDestination());

            _playerAttack._onAttack.Subscribe(_ => { _playerAttackFlag = true; });
        }

        void OnDisable()
        {
            _cts.Cancel();
        }
    
        // Update is called once per frame
        void Update()
        {
            //playerが攻撃した時
            if(_playerAttackFlag && !_searching){
                MoveWhenPlayerAttack().Forget();
            }

            //通常時
            if (!_navMeshAgent.pathPending && !_navMeshAgent.hasPath && !_searching && _enemyBase._startGameFlag) {
                Move(_cts.Token).Forget();
            }
            
        }

        async UniTaskVoid MoveWhenPlayerAttack()
        {
            _searching = true;
            NavMeshHit navMeshHit;
            if(NavMesh.SamplePosition(_playerTransform.position, out navMeshHit, _navMeshAgent.height * 2, 1)) {
                _navMeshAgent.SetDestination(navMeshHit.position);
            }
            while(true){
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: _cts.Token);
                if(!_navMeshAgent.hasPath){
                    await _enemyAttack.AttackOnce();
                    _searching = false;
                    break;
                }
            }

        }

        async UniTaskVoid Move(CancellationToken token)
        {
            _searching = true;
            while(true){
                bool searchFlag = await SearchAround(token);
                if(searchFlag){
                    NavMeshHit navMeshHit;
                    if(NavMesh.SamplePosition(_playerTransform.position, out navMeshHit, _navMeshAgent.height * 2, 1)) {
                        _navMeshAgent.SetDestination(navMeshHit.position);
                    }
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: _cts.Token);
                    if(!_navMeshAgent.hasPath) {
                        if(_enemyAttack) await _enemyAttack.AttackOnce();
                    }
                    SetDestination();
                    _searching = false;
                    break;
                }else{
                    SetDestination();
                    _searching = false;
                    break;
                }
            }
        }

        void SetDestination()
        {
            // NavMesh.SamplePositionが範囲外の場合、正しい場所を取得できない
            for(int i = 0; i < 30; i++){
                Vector3 sourcePosition = _areaBoxPos + new Vector3(
                    Random.Range(-_areaBox.Size.x / 2, _areaBox.Size.x / 2),
                    Random.Range(-_areaBox.Size.y / 2, _areaBox.Size.y / 2),
                    Random.Range(-_areaBox.Size.z / 2, _areaBox.Size.z / 2));
                Debug.Log(sourcePosition);

                NavMeshHit navMeshHit;
                if(NavMesh.SamplePosition(sourcePosition, out navMeshHit, _navMeshAgent.height * 2, 1)) {
                    _navMeshAgent.SetDestination(navMeshHit.position);
                    break;
                }
            }
        }

        async UniTaskVoid LoopHamon(CancellationToken token)
        {
            while(true){
                await UniTask.Delay(System.TimeSpan.FromSeconds(_hamonInterval), cancellationToken: token);
                _effectManager.Play("2D Hamon", transform.position);
            }
        }

        async UniTask<bool> SearchAround(CancellationToken token)
        {
            float rotationAmount = 0;
            Vector3 targetDir = _playerTransform.position - transform.position;
            float targetDistance = targetDir.magnitude;

            while(true) {
                await UniTask.Yield(PlayerLoopTiming.Update);
                float _anglePerFrame = _anglePerSecond * Time.deltaTime;
                rotationAmount += _anglePerFrame;
                transform.rotation = Quaternion.AngleAxis(_anglePerFrame, transform.up) * transform.rotation;
                // search player
                float cosHalf = Mathf.Cos(_sightAngle / 2 * Mathf.Deg2Rad);
                float dot = Vector3.Dot(transform.forward, targetDir.normalized);
                // found
                if(dot > cosHalf && targetDistance < _maxDistance) return true;
                // finish 'couse rotate around
                if(360 <= rotationAmount) return false;
            }
        }
    }
}
