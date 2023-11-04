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

        private IEnumerator AttackCoroutine()
        {
            while (CheckDistance())
            {
                Attack();
                //wait attackDelay
                yield return new WaitForSeconds(1);
            }

            _switcher.SwitchState<ChaseState>();
        }
    }
}