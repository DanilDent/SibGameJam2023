using UnityEngine;

namespace Enemy
{
    public class EnemyPhysics : MonoBehaviour
    {
        [field: SerializeField] public Rigidbody2D RB { get; private set; }
    }
}