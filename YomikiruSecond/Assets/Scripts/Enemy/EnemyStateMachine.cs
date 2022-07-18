using UnityEngine;

namespace Yomikiru.Character.Enemy.StateMachine
{
    public sealed class EnemyStateMachine<T> where T : MonoBehaviour 
    {
        private EnemyState<T> currentState;

        public EnemyState<T> CurrentState
        {
            get => currentState;
            set
            {
                currentState?.OnExit();
                currentState = value;
                currentState?.OnEnter();
            }
        }

        public void Update()
        {
            currentState?.Update();
        }

    }

}
