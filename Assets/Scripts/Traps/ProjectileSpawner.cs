using UnityEngine;
using GameTime;
using Enemy;

namespace Trap
{
    public class ProjectileSpawner : MonoBehaviour
    {
        [SerializeField] private int _ticksCountToActivate;
        [SerializeField] private ProjectileTrap _prefab;

        private int _currentTicksCountToActivate;
        
        private void Start()
        {
            EventBusSingleton.Instance.Subscribe<ClockStepSignal>(OnClockTick);
        }

        private void OnDestroy()
        {
            EventBusSingleton.Instance.Unsubscribe<ClockStepSignal>(OnClockTick);
        }

        private void OnClockTick(ClockStepSignal signal)
        {
            _currentTicksCountToActivate++;

            if (_currentTicksCountToActivate == _ticksCountToActivate)
            {
                _currentTicksCountToActivate = 0;
                Instantiate(_prefab, transform);
            }
        }
    }
}


