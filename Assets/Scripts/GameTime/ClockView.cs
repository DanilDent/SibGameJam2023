using DG.Tweening;
using Enemy;
using UnityEngine;

namespace GameTime
{
    public class ClockView : MonoBehaviour
    {

        [SerializeField] private RectTransform _minuteHand;
        private GameTime _gameTime;
        private EventBusSingleton _eventBus;

        private void Start()
        {
            _eventBus = EventBusSingleton.Instance;
            _gameTime = GameTime.Instance;

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
            sequence.Append(transform.DOScale(1.5f, duration: 0.132f).SetEase(Ease.InCirc));
            sequence.Append(transform.DOScale(1f, duration: 0.132f).SetEase(Ease.OutCirc));
        }

        private void OnClockStep(ClockStepSignal signal)
        {
            if (signal.Step == 0)
                return;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(1.15f, duration: 0.1f).SetEase(Ease.InCirc));
            sequence.Append(transform.DOScale(1f, duration: 0.1f).SetEase(Ease.OutCirc));
        }
    }
}
