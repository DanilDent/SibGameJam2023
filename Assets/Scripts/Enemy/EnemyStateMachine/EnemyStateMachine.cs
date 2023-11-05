using UnityEngine;
using JHelpers;

namespace Enemy
{
    public class EnemyStateMachine : BaseStateMachineOnSwitchers
    {
        public float CheckDistance = 2f;

        public void InitEnemyStateMachine(EnemyContainer enemyContainer, Player.Player player)
        {
            _states.Add(new ChaseState(this, enemyContainer.EnemyAI, player.transform, CheckDistance, enemyContainer.EnemyView));
            _states.Add(new AttackState(this, enemyContainer.EnemyView.EnemyLogic, enemyContainer.EnemyPhysics, player, CheckDistance));
            _currentState = _states[0];
            _currentState.Enter();
        }

        private void FixedUpdate()
        {
            _currentState.FixedUpdate();
        }
    }
}