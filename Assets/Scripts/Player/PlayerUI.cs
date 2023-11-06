using UnityEngine;
using UnityEngine.UI;
using Enemy;

namespace Player
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private Slider _hpBar;

        private void Start()
        {
            EventBusSingleton.Instance.Subscribe<TakeDamage>(OnDamageTaken);
            EventBusSingleton.Instance.Subscribe<Die>(OnDie);
        }

        private void OnDestroy()
        {
            EventBusSingleton.Instance.Unsubscribe<TakeDamage>(OnDamageTaken);
            EventBusSingleton.Instance.Unsubscribe<Die>(OnDie);
        }

        private void OnDamageTaken(TakeDamage signal)
        {
            _hpBar.value = signal.CurrentHealth;
        }

        private void OnDie(Die signal)
        {
            //do smth
        }
    }
}