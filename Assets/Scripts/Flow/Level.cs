using UnityEngine;
using Enemy;
using GameTime;

namespace GameFlow
{
    public class Level : MonoBehaviour
    {
        [SerializeField] private int _fullTicksCountToComplete;
        [field: SerializeField] public EnemySpawner _levelSpawner;

        private int _fullTicksCurrentCount;

        private void Start()
        {
            EventBusSingleton.Instance.Subscribe<ClockFullTurnSignal>(OnFullTurnSignal);
        }

        private void OnDestroy()
        {
            EventBusSingleton.Instance.Unsubscribe<ClockFullTurnSignal>(OnFullTurnSignal);
        }

        private void OnFullTurnSignal(ClockFullTurnSignal signal)
        {
            _fullTicksCurrentCount++;

            if(_fullTicksCurrentCount >= _fullTicksCountToComplete)
                EventBusSingleton.Instance.Invoke(new LevelComplete(this));
        }
    }
}