using Config;
using DG.Tweening;
using Enemy;
using Game;
using UnityEngine;

namespace GameTime
{
    public class ClockView : MonoBehaviour
    {
        private GameTimeSO _gameTimeConfig;

        [SerializeField] private RectTransform _minuteHand;
        [SerializeField] private RectTransform _hourHand;
        private GameTime _gameTime;
        private EventBusSingleton _eventBus;

        private void Start()
        {
            _eventBus = EventBusSingleton.Instance;
            _gameTime = GameTime.Instance;
            _gameTimeConfig = ConfigContainer.Instance.Value.GameTime;

            _eventBus.Subscribe<ClockFullTurnSignal>(OnClockFullTurn);
            _eventBus.Subscribe<ClockStepSignal>(OnClockStep);
        }

        private void OnDestroy()
        {
            _eventBus.Unsubscribe<ClockFullTurnSignal>(OnClockFullTurn);
            _eventBus.Unsubscribe<ClockStepSignal>(OnClockStep);
        }

        private void Update()
        {
            _minuteHand.transform.rotation = Quaternion.Euler(0f, 0f, -(_gameTime.TurnRatio * 360f));
        }

        private void OnClockFullTurn(ClockFullTurnSignal signal)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(1.3f, duration: _gameTimeConfig.EpsSec).SetEase(Ease.InCirc));
            sequence.Append(transform.DOScale(1f, duration: _gameTimeConfig.EpsSec).SetEase(Ease.OutCirc));

            float angle = (_gameTime.FullTurns % 12) * (-360f / 12f);
            _hourHand.transform.DORotate(new Vector3(0f, 0f, angle), duration: 0.25f);
        }

        private void OnClockStep(ClockStepSignal signal)
        {
            if (signal.Step == 0)
                return;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(1.15f, duration: _gameTimeConfig.EpsSec).SetEase(Ease.InCirc));
            sequence.Append(transform.DOScale(1f, duration: _gameTimeConfig.EpsSec).SetEase(Ease.OutCirc));
        }
    }
}
