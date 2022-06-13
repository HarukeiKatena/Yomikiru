using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
using Cysharp.Threading.Tasks;
//using System.Threading.Tasks;
using System.Threading;
using Yomikiru.Effect;

namespace Enemy
{
    public class AIEnemyMove : MonoBehaviour
    {
        [SerializeField] private PlayerManagement playerManager;
        public PlayerManagement PlayerManager
        {
            set { playerManager = value; }
        }

        [SerializeField] private EffectManager effectManager;
        public EffectManager EffectManager
        {
            set { effectManager = value; }
        }

        [SerializeField] private Player.Attack playerAttack;
        [SerializeField] private Transform playerTransform;

        [SerializeField] private NavMeshAgent navMeshAgent;

        [SerializeField] private AreaBox areaBox;

        [SerializeField] private float hamonInterval;

        [SerializeField] private float aroundTime;
        [SerializeField] private float sightAngle;
        [SerializeField] private float maxDistance;

        [SerializeField] private Yomikiru.MapInfo mapInfo;

        private AIEnemyBase enemyBase;
        private AIEnemyAttack enemyAttack;

        private Vector3 areaBoxPos;
        private bool searching;

        [SerializeField] private bool playerAttackFlag;

        private float anglePerSecond;

        private CancellationTokenSource cts;

        // Start is called before the first frame update
        void Start()
        {
            areaBox = mapInfo.AreaBox;

            cts = new CancellationTokenSource();

            enemyBase = GetComponent<AIEnemyBase>();
            enemyAttack = GetComponent<AIEnemyAttack>();

            var player = playerManager.Character[0];
            playerTransform = player.GetComponent<Transform>();
            playerAttack = player.GetComponent<Player.Attack>();
            areaBoxPos = areaBox.GetComponent<Transform>().position;

            anglePerSecond = 360 / aroundTime;
            searching = false;
            enemyBase.OnStartGame.Subscribe(_ => LoopHamon(cts.Token).Forget());
            enemyBase.OnStartGame.Subscribe(_ => SetDestination());

            playerAttack._onAttack.Subscribe(_ => { playerAttackFlag = true; });
        }

        void OnDisable()
        {
            cts.Cancel();
        }

        // Update is called once per frame
        void Update()
        {
            //playerが攻撃した時
            if (playerAttackFlag && !searching)
            {
                MoveWhenPlayerAttack().Forget();
            }

            //通常時
            if (!navMeshAgent.pathPending && !navMeshAgent.hasPath && !searching && enemyBase.StartGameFlag)
            {
                Move(cts.Token).Forget();
            }

        }

        async UniTaskVoid MoveWhenPlayerAttack()
        {
            searching = true;
            NavMeshHit navMeshHit;
            if (NavMesh.SamplePosition(playerTransform.position, out navMeshHit, navMeshAgent.height * 2, 1))
            {
                navMeshAgent.SetDestination(navMeshHit.position);
            }
            while (true)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: cts.Token);
                if (!navMeshAgent.hasPath)
                {
                    await enemyAttack.AttackOnce();
                    searching = false;
                    break;
                }
            }

        }

        async UniTaskVoid Move(CancellationToken token)
        {
            searching = true;
            while (true)
            {
                bool searchFlag = await SearchAround(token);
                if (searchFlag)
                {
                    NavMeshHit navMeshHit;
                    if (NavMesh.SamplePosition(playerTransform.position, out navMeshHit, navMeshAgent.height * 2, 1))
                    {
                        navMeshAgent.SetDestination(navMeshHit.position);
                    }
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: cts.Token);
                    if (!navMeshAgent.hasPath)
                    {
                        if (enemyAttack) await enemyAttack.AttackOnce();
                    }
                    SetDestination();
                    searching = false;
                    break;
                }
                else
                {
                    SetDestination();
                    searching = false;
                    break;
                }
            }
        }

        void SetDestination()
        {
            // NavMesh.SamplePositionが範囲外の場合、正しい場所を取得できない
            for (int i = 0; i < 30; i++)
            {
                Vector3 sourcePosition = areaBoxPos + new Vector3(
                    Random.Range(-areaBox.Size.x / 2, areaBox.Size.x / 2),
                    Random.Range(-areaBox.Size.y / 2, areaBox.Size.y / 2),
                    Random.Range(-areaBox.Size.z / 2, areaBox.Size.z / 2));
                Debug.Log(sourcePosition);

                NavMeshHit navMeshHit;
                if (NavMesh.SamplePosition(sourcePosition, out navMeshHit, navMeshAgent.height * 2, 1))
                {
                    navMeshAgent.SetDestination(navMeshHit.position);
                    break;
                }
            }
        }

        async UniTaskVoid LoopHamon(CancellationToken token)
        {
            while (true)
            {
                await UniTask.Delay(System.TimeSpan.FromSeconds(hamonInterval), cancellationToken: token);
                effectManager.Play("2D Hamon", transform.position);
            }
        }

        async UniTask<bool> SearchAround(CancellationToken token)
        {
            float rotationAmount = 0;
            Vector3 targetDir = playerTransform.position - transform.position;
            float targetDistance = targetDir.magnitude;

            while (true)
            {
                await UniTask.Yield(PlayerLoopTiming.Update);
                float _anglePerFrame = anglePerSecond * Time.deltaTime;
                rotationAmount += _anglePerFrame;
                transform.rotation = Quaternion.AngleAxis(_anglePerFrame, transform.up) * transform.rotation;
                // search player
                float cosHalf = Mathf.Cos(sightAngle / 2 * Mathf.Deg2Rad);
                float dot = Vector3.Dot(transform.forward, targetDir.normalized);
                // found
                if (dot > cosHalf && targetDistance < maxDistance) return true;
                // finish 'couse rotate around
                if (360 <= rotationAmount) return false;
            }
        }
    }
}
