using UnityEngine;
using DG.Tweening;

namespace Enemy
{
    public class EnemyView : MonoBehaviour
    {
        [field: SerializeField] public SpriteRenderer Sprite { get; private set; }

        [field: SerializeField] public float FadeDuraion { get; private set; }

        public EnemyLogic EnemyLogic { get; private set; }

        //use in spawner
        public void Init(EnemyLogic enemyLogic)
        {
            EnemyLogic = enemyLogic;
            SubsribeOnEvents();
        }
        
        private void SubsribeOnEvents()
        {
            EventBusSingleton.Instance.Subscribe<Die>(OnDie);
            EventBusSingleton.Instance.Subscribe<TakeDamage>(OnDamageTaken);
        }

        private void UnsubscribeFromEvents()
        {

            EventBusSingleton.Instance.Unsubscribe<Die>(OnDie);
            EventBusSingleton.Instance.Unsubscribe<TakeDamage>(OnDamageTaken);
        }

        //some ui & anim logic
        private void OnDie(Die signal)
        {
            if (signal.Enemy == EnemyLogic)
            {
                Sprite.DOFade(0, FadeDuraion);
                UnsubscribeFromEvents();
            }
        }

        private void OnDamageTaken(TakeDamage signal)
        {
            if (signal.Enemy == EnemyLogic)
            {
                Sprite.DOColor(Color.red, .5f).OnComplete(() => Sprite.color = Color.white);
            }
        }
    }
}