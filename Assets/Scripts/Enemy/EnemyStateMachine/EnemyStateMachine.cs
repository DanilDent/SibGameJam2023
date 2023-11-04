using UnityEngine;
using JHelpers;

namespace Enemy
{
    public class EnemyStateMachine : BaseStateMachineOnSwitchers
    {
        [SerializeField] private float _checkDistance = 2f;

        public void InitEnemyStateMachine(EnemyContainer enemyContainer, Player.Player player)
        {
            _states.Add(new ChaseState(this, enemyContainer.EnemyAI, player.transform, _checkDistance));
            _states.Add(new AttackState(this, enemyContainer.EnemyView.EnemyLogic, enemyContainer.EnemyPhysics, player, _checkDistance));
            _currentState = _states[0];
            _currentState.Enter();
        }

        private void FixedUpdate()
        {
            _currentState.FixedUpdate();
        }
    }
}