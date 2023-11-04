using Config;
using Enemy;
using Game;
using Helpers;
using Sound;
using System;
using UnityEngine;

namespace GameTime
{
    public class GameTime : MonoSingleton<GameTime>
    {
        private GameConfigSO _config;

        private void Start()
        {
            _eventBus = EventBusSingleton.Instance;
            _config = ConfigContainer.Instance.Value;
            _config.GameTime.Init(this);
            foreach (var trackSO in _config.Tracks)
            {
                trackSO.Init(this);
            }

            Fill(_config);
            Restart();
        }

        public void Restart()
        {
            var track = MusicController.Instance.SetTrack(0);
            _clockFullTurnSec = track.ClockFullTurnSec;
            MusicController.Instance.Play();
            StartGameClock();
        }

        public void Fill(GameConfigSO config)
        {
            _numberOfSteps = config.GameTime.NumberOfSteps;
            _eps = config.GameTime.EpsSec;

            var track = MusicController.Instance.GetCurrentTrack();
            _clockFullTurnSec = track.ClockFullTurnSec;
        }

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

        private void StartGameClock()
        {
            _fullTurns = 0;
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
            int fullTurns = Mathf.FloorToInt(_gameTimeSec / _clockFullTurnSec);
            float currentTurnTime = _gameTimeSec - _fullTurns * _clockFullTurnSec;

            float[] stepTimes = new float[_numberOfSteps];
            for (int i = 0; i < _numberOfSteps; ++i)
            {
                stepTimes[i] = (_clockFullTurnSec / _numberOfSteps) * (_numberOfSteps - i);
            }

            float closestStepTime = stepTimes[0];
            float decimPart = float.MaxValue;
            for (int i = 0; i < _numberOfSteps; ++i)
            {
                float curDecimPart = Mathf.Abs(stepTimes[i] - currentTurnTime);
                if (curDecimPart < decimPart)
                {
                    decimPart = curDecimPart;
                    closestStepTime = stepTimes[i];
                }
            }

            float decimalPartCurrentTime = Mathf.Abs(closestStepTime - currentTurnTime);
            if (decimalPartCurrentTime < _eps)
            {
                int step = Array.IndexOf(stepTimes, closestStepTime);
                if (step == 0)
                {
                    _eventBus.Invoke(new ClockFullTurnSignal());
                }
                _eventBus.Invoke(new ClockStepSignal(step));
            }

            _fullTurns = fullTurns;
        }
    }
}
