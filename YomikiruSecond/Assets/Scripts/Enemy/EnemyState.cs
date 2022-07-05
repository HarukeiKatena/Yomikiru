using System.Collections;
using System.Collections.Generic;

namespace Yomikiru.Enemy
{

    public abstract class EnemyState
    {

        protected AIEnemyBase enemy;

        protected EnemyState(AIEnemyBase enemy) => this.enemy = enemy;

        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void Update();

    }

}
