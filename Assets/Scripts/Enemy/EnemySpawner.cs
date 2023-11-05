using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using JHelpers;
using GameTime;

namespace Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Pool settings")]
        [SerializeField] private EnemyContainer _prefab;
        [SerializeField] private int _count;
        
        [Space(20)]
        [SerializeField] private List<EnemyConfig> _enemySpawnSuquance;
        [SerializeField] private List<Transform> _wayPoints;
        [Space(20)]
        [SerializeField] private Player.Player _player;

        private int _spawnId;
        private int _wayPointId;
        private Dictionary<EnemyLogic, EnemyContainer> _spawnEnemiesDic = new();

        private ObjectPool<EnemyContainer> _objectPool;

        private void Start()
        {
            _objectPool = new ObjectPool<EnemyContainer>(_prefab, _count, true);
            _objectPool.Init(Vector3.zero, Quaternion.identity, transform);
            EventBusSingleton.Instance.Subscribe<ClockFullTurnSignal>(OnClockFullTurn);
            EventBusSingleton.Instance.Subscribe<Die>(OnEnemyDie);
        }

        private void OnDestroy()
        {
            EventBusSingleton.Instance.Unsubscribe<ClockFullTurnSignal>(OnClockFullTurn);
            EventBusSingleton.Instance.Unsubscribe<Die>(OnEnemyDie);
        }

        private void InitEnemy(EnemyContainer enemy, EnemyLogic enemyLogic)
        {
            enemy.EnemyView.Init(enemyLogic);
            enemy.EnemySpriteRotator.Init(enemy.EnemyView.Sprite, enemy.EnemyPhysics.RB, _player.transform);
            enemy.EnemyAI.Init(enemy.Seeker, enemy.EnemyPhysics.RB, _player.transform, enemy.EnemyView.EnemyLogic.Config.MoveSpeed);
            enemy.StateMachine.InitEnemyStateMachine(enemy, _player);
        }

        private void OnClockFullTurn(ClockFullTurnSignal signal)
        {
            SpawnEnemy();
        }

        private void OnEnemyDie(Die signal)
        {
            var container = _spawnEnemiesDic[signal.Enemy];
            container.ChangeActiveStatus(false);
            StartCoroutine(DieCoroutine(container));
        }

        private IEnumerator DieCoroutine( EnemyContainer enemy)
        {
            yield return new WaitForSeconds(enemy.EnemyView.FadeDuraion);
            DespawnEnemy(enemy);
        }

        public void SpawnEnemy()
        {
            if(_spawnId >= _enemySpawnSuquance.Count)
                _spawnId = 0;

            if (_wayPointId >= _wayPoints.Count)
                _wayPointId = 0;

            EnemyLogic enemyLogic = new(_enemySpawnSuquance[_spawnId]);
            var enemy = _objectPool.ActivateObject();
            enemy.transform.position = _wayPoints[_wayPointId].position;
            InitEnemy(enemy, enemyLogic);
            _spawnEnemiesDic.Add(enemyLogic, enemy);

            _spawnId++;
            _wayPointId++;
        }

        public void DespawnEnemy(EnemyContainer enemy)
        {
            _objectPool.DeactivateObject(enemy);
        }
    }
}