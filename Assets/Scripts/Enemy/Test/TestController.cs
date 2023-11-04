using UnityEngine;

namespace Enemy
{
    public class TestController : MonoBehaviour
    {
        [SerializeField] private EnemySpawner _spawner;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                _spawner.SpawnEnemy();
            }
        }
    }
}
