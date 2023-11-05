using UnityEngine;

namespace Enemy
{
    //add hashcode or override Equal for perfonce
    public class EnemyLogic
    {
        public EnemyConfig Config { get; private set; }

        public int CurrentHealth { get; private set; }

        public EnemyLogic(EnemyConfig config)
        {
            Config = config;
            CurrentHealth = Config.Health;
        }

        public void TakeDamage(int damage)
        {
            if (damage <= 0)
                throw new System.Exception("damage less than 0");

            CurrentHealth -= damage;
            EventBusSingleton.Instance.Invoke(new TakeDamage(this, damage));

            if (CurrentHealth <= 0)
            {
                EventBusSingleton.Instance.Invoke(new Die(this));
            }

        }

        //in param must be Player
        public void Attack(Player.Player player)
        {
            player.TakeDamage(Config.Damage);
        }
    }
}