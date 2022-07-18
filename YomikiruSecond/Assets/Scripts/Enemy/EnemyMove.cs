using UnityEngine;
using UnityEngine.AI;

namespace Yomikiru.Character.Enemy
{
    public class EnemyMove : MonoBehaviour
    {
        private Character character;
        private CharacterData table => character.Table;
        private CharacterController controller;
        private NavMeshAgent navMeshAgent;

        private AIEnemyBase aiEnemyBase;

        // search around parameter
        [SerializeField] private float aroundTime;
        [SerializeField] private float sightAngle;
        [SerializeField] private float maxDistance;
        private float degreesPerSecond;

        // map info
        [SerializeField] private AreaBox areaBox;

        private void Awake()
        {
            TryGetComponent(out character);
            TryGetComponent(out controller);
            TryGetComponent(out navMeshAgent);

            degreesPerSecond = 360 / aroundTime;
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

        public bool GetAgentIsStopped() { return navMeshAgent.isStopped; }

        public void SetRandomDestination()
        {
            // NavMesh.SamplePositionが範囲外の場合、正しい場所を取得できない
            for (int i = 0; i < 30; i++)
            {
                Vector3 sourcePosition = areaBox.transform.position + new Vector3(
                    UnityEngine.Random.Range(-areaBox.Size.x / 2, areaBox.Size.x / 2),
                    UnityEngine.Random.Range(-areaBox.Size.y / 2, areaBox.Size.y / 2),
                    UnityEngine.Random.Range(-areaBox.Size.z / 2, areaBox.Size.z / 2));

                if (NavMesh.SamplePosition(sourcePosition, out var navMeshHit, navMeshAgent.height * 2, 1))
                {
                    navMeshAgent.SetDestination(navMeshHit.position);
                    break;
                }
            }
        }

        public void SetDestination(Vector3 playerPosition)
        {
            if (NavMesh.SamplePosition(playerPosition, out var navMeshHit, navMeshAgent.height * 2, 1))
            {
                navMeshAgent.SetDestination(navMeshHit.position);
            }
        }
    }
}
