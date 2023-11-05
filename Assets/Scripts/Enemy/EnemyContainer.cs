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
        [field: SerializeField] public EnemySpriteRotator EnemySpriteRotator;

        private void OnEnable()
        {
            ChangeActiveStatus(true);
        }

        public void ChangeActiveStatus(bool status)
        {
            EnemyView.GetComponent<Collider2D>().enabled = status;
            StateMachine.enabled = status;
            EnemyAI.enabled = status;
        }
    }
}