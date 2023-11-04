using JHelpers;

namespace Enemy
{
    public class EnemySignal : ISignal
    {
        public readonly EnemyLogic Enemy;

        public EnemySignal(EnemyLogic enemy)
        {
            Enemy = enemy;
        }
    }
}