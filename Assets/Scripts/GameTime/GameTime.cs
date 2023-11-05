using Config;
using Enemy;
using Game;
using Helpers;
using Sound;
using System;
using UnityEngine;

namespace GameTime
{
    [DefaultExecutionOrder(-1)]
    public class GameTime : MonoSingleton<GameTime>
    {
        private GameConfigSO _config;

        public bool IsHitBeat
        {
            get
            {
                float currentTurnTime = _gameTimeSec;
                float[] stepTimes = new float[_numberOfSteps];
                for (int i = 0; i < _numberOfSteps; ++i)
                {
                    stepTimes[i] = (_clockFullTurnSec / _numberOfSteps) * (_numberOfSteps - i) + _fullTurns * _clockFullTurnSec;
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

                return Mathf.Abs(closestStepTime - currentTurnTime) < _eps;
            }
        }

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

            float[] stepTimes = new float[_numberOfSteps];
            for (int i = 0; i < _numberOfSteps; ++i)
            {
                stepTimes[i] = (_clockFullTurnSec / _numberOfSteps) * (_numberOfSteps - i) + _fullTurns * _clockFullTurnSec;
                Debug.Log($"StepTime: {stepTimes[i]}, -eps: {stepTimes[i] - _eps}, +eps: {stepTimes[i] + _eps}");
            }

            Debug.Log($"==================================== Я ЕБАЛ ЭТУ ХУЙНЮ =========================================" +
                "===============================================================================================" +
                "=================================================================================================");
        }

        public void Restart()
        {
            var track = MusicController.Instance.SetTrack(0);
            _clockFullTurnSec = track.ClockFullTurnSec;
            MusicController.Instance.Play();
            if (track.OffsetSec > 0f)
            {
                StartCoroutine(Helpers.CoroutineHelpers.InvokeWithDelay(() =>
                {
                    StartGameClock();
                }, track.OffsetSec));
            }
            else
            {
                StartGameClock();
            }
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
        private bool _isEventFired;
        private bool _isClockStepEnter;
        private bool _isClockStepExit;

        private void StartGameClock()
        {
            _fullTurns = 0;
            _gameTimeSec = 0f;
            _startTimeSec = Time.time;
            _isClockStarted = true;
        }

        private void FixedUpdate()
        {
            if (!_isClockStarted)
            {
                return;
            }

            _gameTimeSec = Time.time - _startTimeSec;
            CheckForSteps();
        }

        private float _debguPrevStepTime = -1f;
        private void CheckForSteps()
        {
            _fullTurns = Mathf.FloorToInt(_gameTimeSec / _clockFullTurnSec);
            float currentTurnTime = _gameTimeSec;

            float[] stepTimes = new float[_numberOfSteps];
            for (int i = 0; i < _numberOfSteps; ++i)
            {
                stepTimes[i] = (_clockFullTurnSec / _numberOfSteps) * (_numberOfSteps - i) + _fullTurns * _clockFullTurnSec;
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
            int step = Array.IndexOf(stepTimes, closestStepTime);

            Debug.Log($"game time: {currentTurnTime}");

            float decimalPartCurrentTime = Mathf.Abs(closestStepTime - currentTurnTime);
            if (currentTurnTime > closestStepTime - _eps && !_isClockStepEnter)
            {
                _eventBus.Invoke(new ClockStepEnterSignal(step));
                Debug.Log($"CLOCK STEP TIME DIFF: {currentTurnTime - (_debguPrevStepTime > 0f ? _debguPrevStepTime : 0)}");
                _debguPrevStepTime = currentTurnTime;
                _isClockStepEnter = true;
                _isClockStepExit = false;
                _isEventFired = false;
            }

            if (currentTurnTime > closestStepTime + _eps && !_isClockStepExit)
            {
                _eventBus.Invoke(new ClockStepExitSignal(step));
                Debug.Log($"CLOCK STEP TIME DIFF: {currentTurnTime - (_debguPrevStepTime > 0f ? _debguPrevStepTime : 0)}");
                _debguPrevStepTime = currentTurnTime;
                _isClockStepEnter = false;
                _isClockStepExit = true;
            }

            float eps = 0.05f;
            if (Mathf.Abs(currentTurnTime - closestStepTime) < eps && !_isEventFired)
            {
                if (step == 0)
                {
                    _eventBus.Invoke(new ClockFullTurnSignal());
                    _isEventFired = true;
                }
                else
                {
                    _eventBus.Invoke(new ClockStepSignal(step));
                    _isEventFired = true;
                }
            }

            //_fullTurns = fullTurns;
        }
    }
}
