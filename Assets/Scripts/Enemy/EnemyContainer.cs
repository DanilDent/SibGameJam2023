using Pathfinding;
using UnityEngine;

namespace Enemy
{
    public class EnemyContainer : MonoBehaviour
    {
        [field: SerializeField] public EnemyPhysics EnemyPhysics;
        [field: SerializeField] public EnemyView EnemyView;
        [field: SerializeField] public EnemyStateMachine StateMachine;
        [field: SerializeField] public EnemyAI EnemyAI;
        [field: SerializeField] public Seeker Seeker;
    }
}