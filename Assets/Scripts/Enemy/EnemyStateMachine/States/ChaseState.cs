using UnityEngine;
using System.Collections;
using JHelpers;

namespace Enemy
{
    public class ChaseState : BaseStateOnSwitcher
    {
        private Transform _target;
        private EnemyPhysics _enemyPhysics;
        private float _enemyMoveSpeed;

        public ChaseState(IStateSwitcher switcher, Transform target, EnemyPhysics enemy, float enemyMoveSpeed) : base(switcher)
        {
            _enemyMoveSpeed = enemyMoveSpeed;
            _enemyPhysics = enemy;
            _target = target;
        }

        public override void Enter() { }

        public override void Exit() { }

        public override void FixedUpdate()
        {
            if (CheckDistance())
            {
                _switcher.SwitchState<AttackState>();
                return;
            }
            
            _enemyPhysics.RB.AddForce((_target.transform.position - _enemyPhysics.transform.position) * _enemyMoveSpeed, ForceMode2D.Force);
        }

        private bool CheckDistance()
        {
            MonoBehaviour.print(Vector3.Distance(_enemyPhysics.transform.position, _target.position));
            return Vector3.Distance(_enemyPhysics.transform.position, _target.position) <= 1f;
        }
    }
}