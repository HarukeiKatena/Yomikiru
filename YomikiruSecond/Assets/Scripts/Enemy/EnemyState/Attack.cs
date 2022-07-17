using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Yomikiru.Character.Enemy.StateMachine;

namespace Yomikiru.Character.Enemy.State
{
    public sealed class Attack : EnemyState<AIEnemyBase>
    {
        bool finish;
        public Attack(AIEnemyBase enemy) : base(enemy)
        {
        }

        public override void OnEnter()
        {
            if(enemy.Attack)
            {
                enemy.Attack.Attack();
                Debug.Log("Attack");
            }
            finish = false;
            WaitSeconds(enemy.AttackTime, enemy.Cts.Token).Forget();
        }

        public override void OnExit()
        {
        }

        public override void Update()
        {
            if(finish)
            {
                enemy.StateMachine.CurrentState = enemy.SearchState;
            }
        }

        private async UniTaskVoid WaitSeconds(float seconds, CancellationToken token)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(seconds), cancellationToken: token);
            finish = true;
        }
    }

}
