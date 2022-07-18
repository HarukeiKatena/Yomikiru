using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Yomikiru.Character.Enemy.StateMachine;

namespace Yomikiru.Character.Enemy.State
{
    public sealed class EnemyStateAttack : EnemyState<AIEnemyBase>
    {
        public EnemyStateAttack(AIEnemyBase enemy) : base(enemy)
        {
        }

        public override void OnEnter()
        {
            if(enemy.Attack)
            {
                enemy.Attack.Attack();
                Debug.Log("Attack");
            }
            ToSearchState(enemy.AttackTime, enemy.GetCancellationTokenOnDestroy()).Forget();
        }

        public override void OnExit()
        {
        }

        public override void Update()
        {
        }

        private async UniTaskVoid ToSearchState(float seconds, CancellationToken token)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(seconds), cancellationToken: token);
            enemy.StateMachine.CurrentState = new EnemyStateSearch(enemy);
        }
    }

}
