using UnityEngine;
using Yomikiru.Character.Enemy;

namespace Yomikiru.Character.Enemy.StateMachine
{

    public abstract class EnemyState<T> where T : MonoBehaviour
    {
        protected T enemy;

        protected EnemyState(T enemy) => this.enemy = enemy;

        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void Update();

    }

}
