using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHelpers;

namespace Enemy
{
    public class EnemyStateMachine : BaseStateMachineOnSwitchers
    {
        //insted Of vector3 use player for runtime culc 
        public void InitEnemyStateMachine(EnemyContainer enemyContainer, Transform player)
        {
            _states.Add(new ChaseState(this, enemyContainer.EnemyAI));
            _states.Add(new AttackState(this, enemyContainer.EnemyView.EnemyLogic, enemyContainer.EnemyPhysics, player));
            _currentState = _states[0];
            _currentState.Enter();
        }

        private void FixedUpdate()
        {
            _currentState.FixedUpdate();
        }
    }
}