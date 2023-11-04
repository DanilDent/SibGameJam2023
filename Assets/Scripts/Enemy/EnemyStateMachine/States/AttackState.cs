using System.Collections;
using UnityEngine;
using JHelpers;

namespace Enemy
{
    public class AttackState : BaseStateOnSwitcher
    {
        //must be player field
        private Player.Player _target;
        private EnemyPhysics _enemyPos;
        private EnemyLogic _enemy;
        private Coroutine _attackRoutine;
        private float _checkDistance;

        public AttackState(IStateSwitcher switcher, EnemyLogic enemy, EnemyPhysics enemyPos, Player.Player target, float checkDistance) : base(switcher)
        {
            _enemyPos = enemyPos;
            _target = target;
            _enemy = enemy;
            _checkDistance = checkDistance;
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
            return Vector2.Distance(_enemyPos.transform.position, _target.transform.position) <= _checkDistance;
        }

        //need player in param
        private void Attack()
        {
            _enemy.Attack(_target);
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