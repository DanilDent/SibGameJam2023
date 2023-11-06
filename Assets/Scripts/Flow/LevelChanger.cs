using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using UnityEngine.SceneManagement;

namespace GameFlow
{
    public class LevelChanger : MonoBehaviour
    {
        [SerializeField] private bool _restartLevelOnPlayerDie;
        [SerializeField] private int _battleSceneIndex = 3;

        [Header("Levels must be in right order")]
        [SerializeField] private List<Level> _levels;
        [SerializeField] private Player.Player _player;

        private Level _currentLevel;
        private int _currentLevelIndex;

        private void Start()
        {
            EventBusSingleton.Instance.Subscribe<LevelComplete>(OnLevelComplete);
            EventBusSingleton.Instance.Subscribe<Player.Die>(OnPlayerDie);
            _currentLevel = Instantiate(_levels[_currentLevelIndex]);
            PathfinderHandler.Instance.BakePath();
        }

        private void OnDestroy()
        {
            EventBusSingleton.Instance.Unsubscribe<LevelComplete>(OnLevelComplete);
            EventBusSingleton.Instance.Unsubscribe<Player.Die>(OnPlayerDie);
        }

        private void OnLevelComplete(LevelComplete signal)
        {
            if (_currentLevelIndex >= _levels.Count)
                return;

            Destroy(_currentLevel.gameObject);
            _currentLevel = Instantiate(_levels[++_currentLevelIndex]);
            PathfinderHandler.Instance.BakePath();
            _player.transform.position = Vector3.zero;
        }

        private void OnPlayerDie(Player.Die signal)
        {
            Debug.LogWarning("Invoke die");

            if (_restartLevelOnPlayerDie)
            {
                Destroy(_currentLevel.gameObject);
                var level = Instantiate(_levels[_currentLevelIndex]);
                PathfinderHandler.Instance.BakePath();
                _currentLevel = level;
                _player.transform.position = Vector3.zero;
                EventBusSingleton.Instance.Invoke(new LevelComplete());
                //restart watch

            }
            else
            {
                SceneManager.LoadScene(_battleSceneIndex);
                //PathfinderHandler.Instance.BakePath();
                //_player.transform.position = Vector3.zero;
            }
        }
    }
}