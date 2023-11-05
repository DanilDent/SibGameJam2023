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
        }

        public override void Exit()
        {
        }

        private bool CheckDistance()
        {
            return Vector2.Distance(_enemyPos.transform.position, _target.transform.position) <= _checkDistance;
        }

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
                if (_attackRoutine != null)
                {
                    _enemyPos.StopCoroutine(_attackRoutine);
                    _attackRoutine = null;
                }

                _switcher.SwitchState<ChaseState>();
            }
        }

        private IEnumerator AttackCoroutine()
        {
            yield return new WaitForSeconds(_enemy.Config.AttackDelay);
            Attack();
            MonoBehaviour.print("attack");
            yield return new WaitForSeconds(_enemy.Config.AttackCD);
            _attackRoutine = null;
        }
    }
}