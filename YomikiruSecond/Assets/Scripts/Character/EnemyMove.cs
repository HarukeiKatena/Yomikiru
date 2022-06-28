using System;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
using Cysharp.Threading.Tasks;

namespace Yomikiru.Character
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyMove : MonoBehaviour
    {
        // イベント（発行）
        private readonly Subject<Vector2> onEnemyMove = new Subject<Vector2>();
        
        // イベント（講読）
        public IObservable<Vector2> OnEnemyMove => onEnemyMove;
        
        // 内部コンポーネント
        private Character character;
        private CharacterData table;
        private CharacterController controller;
        private NavMeshAgent navMeshAgent;

        // 内部パラメーター

        private void Awake()
        {
            TryGetComponent(out character);
            TryGetComponent(out controller);
            TryGetComponent(out navMeshAgent);
        }

        private void Start()
        {
            table = character.Table;
        }

        public async UniTask MoveToPoint(Vector3 dest)
        {
            if (navMeshAgent is null || navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete)
                return;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(dest, out hit, 10, NavMesh.AllAreas) is false)
                return;

            var path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path);
            
            var length = Vector3.Distance(path.corners[path.corners.Length - 1], hit.position);
            if (length > 1.0f)
                return;

            while (Vector3.Distance(navMeshAgent.destination, transform.position) > 1.0f)
            {
                await UniTask.Yield(PlayerLoopTiming.Update);

                Vector3 moveDir = navMeshAgent.steeringTarget - transform.position;

                Quaternion rotDir = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotDir, 120.0f * Time.deltaTime);

                float angle = Vector3.Angle(moveDir, transform.forward);
                if (angle < 30f)
                {
                    
                }
                
                navMeshAgent.SetDestination(hit.position);
                navMeshAgent.nextPosition = transform.position;
            }
        }
    }
}
