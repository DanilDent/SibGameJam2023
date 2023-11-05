using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using GameFlow;
using GameTime;

namespace Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private int _spawnCountPerTick = 1;
        [SerializeField] private bool _spawnOnEvetyPointPerTick;
        [Space(20)]
        [SerializeField] private List<EnemyContainer> _enemySpawnSuquance;
        [SerializeField] private List<Transform> _wayPoints;
        [Space(20)]
        [SerializeField] private Player.Player _player;

        private int _spawnId;
        private int _wayPointId;
        private Dictionary<EnemyLogic, EnemyContainer> _spawnEnemiesDic = new();

        private void Start()
        {
            EventBusSingleton.Instance.Subscribe<ClockFullTurnSignal>(OnClockFullTurn);
            EventBusSingleton.Instance.Subscribe<Die>(OnEnemyDie);
            EventBusSingleton.Instance.Subscribe<LevelComplete>(OnLevelComplete);
        }

        private void OnDestroy()
        {
            EventBusSingleton.Instance.Unsubscribe<ClockFullTurnSignal>(OnClockFullTurn);
            EventBusSingleton.Instance.Unsubscribe<Die>(OnEnemyDie);
            EventBusSingleton.Instance.Unsubscribe<LevelComplete>(OnLevelComplete);
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
            _spawnEnemiesDic.Remove(signal.Enemy);
            container.ChangeActiveStatus(false);
            StartCoroutine(DieCoroutine(container));
        }

        private IEnumerator DieCoroutine(EnemyContainer enemy)
        {
            yield return new WaitForSeconds(enemy.EnemyView.FadeDuraion);
            DespawnEnemy(enemy);
        }

        private void OnLevelComplete(LevelComplete signal)
        {
            foreach(var key in _spawnEnemiesDic.Keys)
            {
                Destroy(_spawnEnemiesDic[key].gameObject);
            }

            _spawnEnemiesDic.Clear();
        }

        private void Spawn()
        {
            if (_spawnId >= _enemySpawnSuquance.Count)
                _spawnId = 0;

            if (_wayPointId >= _wayPoints.Count)
                _wayPointId = 0;

            var enemy = Instantiate(_enemySpawnSuquance[_spawnId], transform);
            EnemyLogic enemyLogic = new(enemy.EnemyConfig);
            enemy.transform.position = _wayPoints[_wayPointId].position;
            InitEnemy(enemy, enemyLogic);
            _spawnEnemiesDic.Add(enemyLogic, enemy);

            _spawnId++;
            _wayPointId++;
        }

        public void SpawnEnemy()
        {
            for (int i = 0; i < _spawnCountPerTick; i++)
            {
                if (_spawnOnEvetyPointPerTick)
                    foreach (var point in _wayPoints)
                        Spawn();
                else
                    Spawn();
            }
        }

        public void DespawnEnemy(EnemyContainer enemy)
        {
            Destroy(enemy.gameObject);
        }
    }
}