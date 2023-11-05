using UnityEngine;
using System.Collections;
using GameTime;
using Enemy;

namespace Trap
{
    public class Trap : MonoBehaviour
    {
        [SerializeField] private int _ticksCountToActivate;
        [SerializeField] private int _damage;

        private int _currentTicksCountToActivate;
        private Animator _animator;
        private Collider2D _collider;
        private float _animLength;
        private Coroutine _animCoroutine;

        private const string CLOSE_TRIGGER = "CloseTrigger";
        private const string OPEN_TRIGGER = "OpenTrigger";

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _collider = GetComponent<Collider2D>();
            EventBusSingleton.Instance.Subscribe<ClockStepSignal>(OnClockTick);
            _animLength = 1f;
            _collider.enabled = false;
        }

        private void OnDestroy()
        {
            EventBusSingleton.Instance.Unsubscribe<ClockStepSignal>(OnClockTick);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Player.Player player))
            {
                player.TakeDamage(_damage);
            }

            if (collision.TryGetComponent(out EnemyView enemy))
            {
                enemy.EnemyLogic.TakeDamage(_damage);
            }

            if (collision.TryGetComponent(out Player.Player _) || collision.TryGetComponent(out EnemyView _))
                if (_animCoroutine == null)
                    _animCoroutine = StartCoroutine(PlayAnim(CLOSE_TRIGGER));
        }

        private IEnumerator PlayAnim(string AnimTrigger)
        {
            _animator.SetTrigger(AnimTrigger);
            yield return new WaitForSeconds(_animLength);
            _collider.enabled = AnimTrigger == OPEN_TRIGGER;
            _animCoroutine = null;
        }

        private void OnClockTick(ClockStepSignal signal)
        {
            _currentTicksCountToActivate++;

            if (_currentTicksCountToActivate == _ticksCountToActivate)
            {
                _currentTicksCountToActivate = 0;
                StartCoroutine(PlayAnim(OPEN_TRIGGER));
            }
        }
    }
}