using UnityEngine;

namespace Player
{
    public class TakeDamage : PlayerSignal
    {
        public readonly int CurrentHealth;

        public TakeDamage(Player player, int currentHealth) : base(player)
        {
            CurrentHealth = currentHealth;
        }
    }
}