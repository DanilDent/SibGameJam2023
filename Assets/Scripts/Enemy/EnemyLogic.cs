using UnityEngine;

namespace Enemy
{
    //add hashcode or override Equal for perfonce
    public class EnemyLogic
    {
        public EnemyConfig Config { get; private set; }

        private int _currentHealth;

        public EnemyLogic(EnemyConfig config)
        {
            Config = config;
            _currentHealth = Config.Health;
        }

        public void TakeDamage(int damage)
        {
            if (damage <= 0)
                throw new System.Exception("damage less than 0");

            _currentHealth -= damage;
            EventBusSingleton.Instance.Invoke(new TakeDamage(this, damage));

            if (_currentHealth <= 0)
            {
                EventBusSingleton.Instance.Invoke(new Die(this));
            }

        }

        //in param must be Player
        public void Attack()
        {
            MonoBehaviour.print("Attacked");
        }
    }
}