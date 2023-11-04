using UnityEngine;
using JHelpers;
using Pathfinding;

namespace Enemy
{
    public class ChaseState : BaseStateOnSwitcher
    {
        private EnemyAI _enemyAI;
        private Transform _playerPos;
        private float _checkDistance;

        public ChaseState(IStateSwitcher switcher, EnemyAI enemy, Transform player, float checkDistance) : base(switcher)
        {
            _enemyAI = enemy;
            _playerPos = player;
            _checkDistance = checkDistance;
        }

        public override void Enter() 
        {
        }

        public override void Exit() { }

        public override void FixedUpdate()
        {
            if (CheckDistance())
                _switcher.SwitchState<AttackState>();

            _enemyAI.UpdatePos();
        }

        private bool CheckDistance()
        {
            return Vector2.Distance(_enemyAI.transform.position, _playerPos.transform.position) <= _checkDistance;
        }
    }
}