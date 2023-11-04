namespace Enemy
{
    public class TakeDamage : EnemySignal
    {
        public readonly int Damage;

        public TakeDamage(EnemyLogic enemy, int damage) : base(enemy)
        {
            Damage = damage;
        }
    }
}