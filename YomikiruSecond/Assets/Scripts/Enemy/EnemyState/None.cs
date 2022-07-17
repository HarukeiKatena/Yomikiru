using System;
using UnityEngine;
using Yomikiru.Character.Enemy.StateMachine;
using UniRx;

namespace Yomikiru.Character.Enemy.State
{
    public sealed class None : EnemyState<AIEnemyBase>
    {
        public None(AIEnemyBase enemy) : base(enemy)
        {
        }

        public override void OnEnter()
        {
        }

        public override void OnExit()
        {
        }

        public override void Update()
        {
        }
    }

}
