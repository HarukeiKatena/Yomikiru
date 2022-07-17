using System;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
using Cysharp.Threading.Tasks;
using Yomikiru.Character.Enemy.StateMachine;
using Yomikiru.Character.Enemy.State;

namespace Yomikiru.Character.Enemy
{
    public class EnemyMove : MonoBehaviour
    {
        // component
        private Character character;
        private CharacterData table;
        private CharacterController controller;
        private NavMeshAgent navMeshAgent;

        private AIEnemyBase aiEnemyBase;

        //serch around perameter
        [SerializeField] private float aroundTime;
        [SerializeField] private float sightAngle;
        [SerializeField] private float maxDistance;
        private float anglePerSecond;

        // map info
        [SerializeField] private AreaBox areaBox;

        private void Awake()
        {
            TryGetComponent(out character);
            TryGetComponent(out controller);
            TryGetComponent(out navMeshAgent);

            anglePerSecond = 360 / aroundTime;
        }

        private void Start()
        {
            table = character.Table;
        }

        private void Update()
        {
        }

        public bool GetReachDestination()
        {
            return !navMeshAgent.hasPath;
        }

        public void StopAgent()
        {
            navMeshAgent.isStopped = true;
        }

        public void RestartAgent()
        {
            navMeshAgent.isStopped = false;
        }

        public void SetRandomDestination()
        {
            // NavMesh.SamplePositionが範囲外の場合、正しい場所を取得できない
            for (int i = 0; i < 30; i++)
            {
                Vector3 sourcePosition = areaBox.transform.position + new Vector3(
                    UnityEngine.Random.Range(-areaBox.Size.x / 2, areaBox.Size.x / 2),
                    UnityEngine.Random.Range(-areaBox.Size.y / 2, areaBox.Size.y / 2),
                    UnityEngine.Random.Range(-areaBox.Size.z / 2, areaBox.Size.z / 2));

                NavMeshHit navMeshHit;
                if (NavMesh.SamplePosition(sourcePosition, out navMeshHit, navMeshAgent.height * 2, 1))
                {
                    navMeshAgent.SetDestination(navMeshHit.position);
                    Debug.Log(navMeshHit.position);
                    break;
                }
            }
        }

        public void SetDestinationForPlayer(Vector3 sourcePosition)
        {
            NavMeshHit navMeshHit;
            if (NavMesh.SamplePosition(sourcePosition, out navMeshHit, navMeshAgent.height * 2, 1))
            {
                navMeshAgent.SetDestination(navMeshHit.position);
            }
        }

        public async UniTask<bool> SearchAround(CancellationToken token)
        {
            float rotationAmount = 0;
            Vector3 targetDir = aiEnemyBase.PlayerAttack.transform.position - transform.position;
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
