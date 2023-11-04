using JHelpers;

namespace Enemy
{
    public class EnemyHited : ISignal
    {
        public readonly EnemyContainer EnemyContainer;

        public EnemyHited(EnemyContainer enemy)
        {
            EnemyContainer = enemy;
        }
    }
}