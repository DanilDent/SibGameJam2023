using UnityEngine;

namespace Player
{
    public class TakeDamage : PlayerSignal
    {
        public readonly int Damage;

        public TakeDamage(Player player, int damage) : base(player)
        {
            Damage = damage;
        }
    }
}