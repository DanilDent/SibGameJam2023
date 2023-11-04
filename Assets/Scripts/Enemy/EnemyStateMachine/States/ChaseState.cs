using UnityEngine;
using JHelpers;
using Pathfinding;

namespace Enemy
{
    public class ChaseState : BaseStateOnSwitcher
    {
        private EnemyAI _enemyAI;

        public ChaseState(IStateSwitcher switcher, EnemyAI enemy) : base(switcher)
        {
            _enemyAI = enemy;
        }

        public override void Enter() 
        {
            MonoBehaviour.print("InChase");
        }

        public override void Exit() { }

        public override void FixedUpdate()
        {
            if (_enemyAI.ReachedEndOfPath)
                _switcher.SwitchState<AttackState>();

            MonoBehaviour.print("chase");
            _enemyAI.UpdatePos();
        }
    }
}