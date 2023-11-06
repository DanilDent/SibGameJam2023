using Config;
using Enemy;
using Game;
using Helpers;
using Sound;
using System;
using System.Collections;
using UnityEngine;

namespace GameTime
{
    [DefaultExecutionOrder(-1)]
    public class GameTime : MonoSingleton<GameTime>
    {
        private GameConfigSO _config;
        private double CurrentBeat;
        private int CurrentBeatNumb;

        public bool IsHitBeat
        {
            get
            {
                return _isClockStepEnter;
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
            StartCoroutine(RestartCoroutine());

            double[] stepTimes = new double[_numberOfSteps];
            for (int i = 0; i < _numberOfSteps; ++i)
            {
                stepTimes[i] = (_clockFullTurnSec / _numberOfSteps) * (_numberOfSteps - i) + _fullTurns * _clockFullTurnSec;
                Debug.Log($"StepTime: {stepTimes[i]}, -eps: {stepTimes[i] - _userEps}, +eps: {stepTimes[i] + _userEps}");
            }

            Debug.Log($"==================================== Я ЕБАЛ ЭТУ ХУЙНЮ =========================================" +
                "===============================================================================================" +
                "=================================================================================================");
        }

        public IEnumerator RestartCoroutine()
        {
            var track = MusicController.Instance.SetTrack(0);
            _clockFullTurnSec = track.ClockFullTurnSec;
            MusicController.Instance.Play();
            StartGameClock();
            yield return new WaitForSecondsRealtime(MusicController.Instance.Audio.clip.length);
            MusicController.Instance.Stop();
            MusicController.Instance.SetTrack(1);
            MusicController.Instance.Play(loop: true);
        }

        public void Fill(GameConfigSO config)
        {
            _numberOfSteps = config.GameTime.NumberOfSteps;
            _userEps = config.GameTime.EpsSec;

            var track = MusicController.Instance.GetCurrentTrack();
            _clockFullTurnSec = track.ClockFullTurnSec;
        }

        [SerializeField] private double _clockFullTurnSec = 4f;
        [SerializeField] private int _numberOfSteps = 4;
        [SerializeField] private double _userEps = 1e-2f;
        private int _fullTurns;

        public double TurnRatio
        {
            get
            {
                int fullTurns = (int)(_gameTimeSec / _clockFullTurnSec);
                double currentTurnTime = _gameTimeSec - fullTurns * _clockFullTurnSec;
                return currentTurnTime / _clockFullTurnSec;
            }
        }

        public int FullTurns => _fullTurns;

        private EventBusSingleton _eventBus;

        private double _startTimeSec;
        private double _gameTimeSec;

        private bool _isClockStarted;
        private bool _isEventFired;
        private bool _isClockStepEnter;
        private bool _isClockStepExit;

        private void StartGameClock()
        {
            _fullTurns = 0;
            _gameTimeSec = 0f;
            _startTimeSec = AudioSettings.dspTime;
            CurrentBeat = 0f;
            CurrentBeatNumb = 0;
            _isClockStarted = true;
        }

        private void Update()
        {
            if (!_isClockStarted)
            {
                return;
            }

            _gameTimeSec = AudioSettings.dspTime - _startTimeSec;
            CheckForSteps();
        }

        private double _debguPrevStepTime = -1f;
        private void CheckForSteps()
        {
            double currentTurnTime = _gameTimeSec;

            double[] stepTimes = new double[_numberOfSteps];
            for (int i = 0; i < _numberOfSteps; ++i)
            {
                stepTimes[i] = (_clockFullTurnSec / _numberOfSteps) * (_numberOfSteps - i) + _fullTurns * _clockFullTurnSec;
            }

            double closestStepTime = stepTimes[0];
            double decimPart = double.MaxValue;
            for (int i = 0; i < _numberOfSteps; ++i)
            {
                double curDecimPart = Math.Abs(stepTimes[i] - currentTurnTime);
                if (curDecimPart < decimPart)
                {
                    decimPart = curDecimPart;
                    closestStepTime = stepTimes[i];
                }
            }
            int step = Array.IndexOf(stepTimes, closestStepTime);

            //Debug.Log($"game time: {currentTurnTime}");

            double decimalPartCurrentTime = Math.Abs(closestStepTime - currentTurnTime);
            double mathEps = 0.02f;

            if (currentTurnTime > closestStepTime - _userEps && currentTurnTime < closestStepTime + _userEps && !_isClockStepEnter)
            {
                // First time come inside current step
                _eventBus.Invoke(new ClockStepEnterSignal(step));
                Debug.Log($"CLOCK STEP TIME ENTER: {_gameTimeSec}, STEP: {step}");
                _debguPrevStepTime = currentTurnTime;
                _isClockStepEnter = true;
                _isClockStepExit = false;
                _isEventFired = false;
            }

            if (Math.Abs(currentTurnTime - closestStepTime) < mathEps && !_isEventFired)
            {
                // We are exactly at current step
                _eventBus.Invoke(new ClockStepSignal(step));
                _isEventFired = true;

                Debug.Log($"CLOCK STEP TIME EVENT: {_gameTimeSec}, STEP: {step}");
            }
            else if (_isClockStepEnter)
            {
                //Debug.Log($"Inside current step: {_gameTimeSec}");
            }

            if (currentTurnTime > closestStepTime + _userEps && !_isClockStepExit && _isClockStepEnter)
            {
                // We leave user range of the current step
                _eventBus.Invoke(new ClockStepExitSignal(step));
                Debug.Log($"CLOCK STEP TIME EXIT: {_gameTimeSec}, STEP: {step}");
                _debguPrevStepTime = currentTurnTime;
                _isClockStepExit = true;
                _isClockStepEnter = false;
                Player.Player.Instance.UserAreadyHitBit = false;
                _fullTurns = (int)(_gameTimeSec / _clockFullTurnSec);
                Debug.Log($"New full turns: {_fullTurns}");
                if (step == 0)
                {
                    _eventBus.Invoke(new ClockFullTurnSignal());
                    _isEventFired = true;
                }
            }
        }
    }
}
