using UnityEngine;

namespace Enemy
{
    public class EnemyView : MonoBehaviour
    {
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
                Debug.Log("enemy die");
                UnsubscribeFromEvents();
            }
        }

        private void OnDamageTaken(TakeDamage signal)
        {
            if (signal.Enemy == EnemyLogic)
            {
                Debug.Log("enemy take damage");
            }
        }
    }
}