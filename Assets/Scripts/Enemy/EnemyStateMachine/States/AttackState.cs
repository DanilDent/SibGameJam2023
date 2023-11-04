using System.Collections;
using UnityEngine;
using JHelpers;

namespace Enemy
{
    public class AttackState : BaseStateOnSwitcher
    {
        //must be player field
        private Transform _target;
        private EnemyPhysics _enemyPos;
        private EnemyLogic _enemy;
        private Coroutine _attackRoutine;

        public AttackState(IStateSwitcher switcher, EnemyLogic enemy, EnemyPhysics enemyPos, Transform target) : base(switcher)
        {
            _enemyPos = enemyPos;
            _target = target;
            _enemy = enemy;
        }

        public override void Enter()
        {
            _enemyPos.StartCoroutine(AttackCoroutine());
        }

        public override void Exit()
        {
            _enemyPos.StopAllCoroutines();
        }

        private bool CheckDistance()
        {
            return Vector2.Distance(_enemyPos.transform.position, _target.position) <= 1f;
        }

        //need player in param
        private void Attack()
        {
            _enemy.Attack();
        }

        public override void FixedUpdate()
        {
            if (CheckDistance())
            {
                if (_attackRoutine == null)
                    _attackRoutine = _enemyPos.StartCoroutine(AttackCoroutine());
            }
            else
            {
                _switcher.SwitchState<ChaseState>();
            }
        }

        private IEnumerator AttackCoroutine()
        {
            Attack();
            yield return new WaitForSeconds(1);
            _attackRoutine = null;

        }
    }
}