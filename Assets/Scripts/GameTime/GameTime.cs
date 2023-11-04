using Enemy;
using Helpers;
using UnityEngine;

namespace GameTime
{
    public class GameTime : MonoSingleton<GameTime>
    {
        [SerializeField] private float _clockFullTurnSec = 4f;
        [SerializeField] private int _numberOfSteps = 4;
        [SerializeField] private float _eps = 1e-2f;
        private int _fullTurns;

        public float TurnRatio
        {
            get
            {
                int fullTurns = Mathf.FloorToInt(_gameTimeSec / _clockFullTurnSec);
                float currentTurnTime = _gameTimeSec - fullTurns * _clockFullTurnSec;
                return currentTurnTime / _clockFullTurnSec;
            }
        }

        public int FullTurns => _fullTurns;

        private EventBusSingleton _eventBus;

        private float _startTimeSec;
        private float _gameTimeSec;

        private bool _isClockStarted;

        private void Start()
        {
            _eventBus = EventBusSingleton.Instance;

            StartGameClock();
        }

        private void StartGameClock()
        {
            _gameTimeSec = 0f;
            _startTimeSec = Time.time;
            _isClockStarted = true;
        }

        private void Update()
        {
            if (!_isClockStarted)
            {
                return;
            }

            _gameTimeSec = Time.time - _startTimeSec;
            CheckForSteps();
        }

        private void CheckForSteps()
        {
            _fullTurns = Mathf.FloorToInt(_gameTimeSec / _clockFullTurnSec);
            float currentTurnTime = _gameTimeSec - _fullTurns * _clockFullTurnSec;

            float decimalPartCurrentTime = currentTurnTime - (int)currentTurnTime;
            if (decimalPartCurrentTime < _eps)
            {
                int step = (int)currentTurnTime;
                if (step == 0)
                {
                    _eventBus.Invoke(new ClockFullTurnSignal());
                }
                _eventBus.Invoke(new ClockStepSignal(step));
            }
        }
    }
}
