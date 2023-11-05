using Enemy;
using UnityEngine;
using GameTime;
using System.Collections;

namespace Trap
{
    public class ProjectileTrap : MonoBehaviour
    {
        [SerializeField] private int _damage;
        [SerializeField] private float _speed;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Player.Player player))
            {
                player.TakeDamage(_damage);
            }

            if (collision.TryGetComponent(out EnemyView enemy))
            {
                enemy.EnemyLogic.TakeDamage(_damage);
            }

            Destroy(gameObject);
        }

        private void Update()
        {
            transform.position += transform.right * Time.deltaTime * _speed;
        }
    }
}