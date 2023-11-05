using Pathfinding;
using UnityEngine;

namespace Enemy
{
    public class EnemyContainer : MonoBehaviour
    {
        [field: SerializeField] public EnemyPhysics EnemyPhysics { get; private set; }
        [field: SerializeField] public EnemyView EnemyView { get; private set; }
        [field: SerializeField] public EnemyStateMachine StateMachine { get; private set; }
        [field: SerializeField] public EnemyAI EnemyAI { get; private set; }
        [field: SerializeField] public Seeker Seeker { get; private set; }
        [field: SerializeField] public EnemySpriteRotator EnemySpriteRotator { get; private set; }
        [field: SerializeField] public EnemyConfig EnemyConfig { get; private set; } 
        [field: SerializeField] public Collider2D Colider { get; set; } 

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