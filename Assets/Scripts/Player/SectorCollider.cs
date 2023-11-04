using UnityEngine;
using Enemy;

namespace Player
{
    public class SectorCollider : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out EnemyContainer enemy))
                EventBusSingleton.Instance.Invoke(new EnemyHited(enemy));
        }
    }
}