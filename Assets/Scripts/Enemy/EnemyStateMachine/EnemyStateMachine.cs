using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHelpers;

namespace Enemy
{
    public class EnemyStateMachine : BaseStateMachineOnSwitchers
    {
        //insted Of vector3 use player for runtime culc 
        public void InitEnemyStateMachine(Transform target, EnemyPhysics enemyPhysics, EnemyLogic enemyLogic, float moveSpeed)
        {
            _states.Add(new ChaseState(this, target, enemyPhysics, moveSpeed));
            _states.Add(new AttackState(this, enemyLogic, enemyPhysics, target));
            _currentState = _states[0];
            _currentState.Enter();
        }

        private void FixedUpdate()
        {
            _currentState.FixedUpdate();
        }
    }
}