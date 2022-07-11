using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yomikiru.Enemy
{

    public sealed class EnemyStateMachine
    {

        private EnemyState currentState;

        public EnemyState CurrentState
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
