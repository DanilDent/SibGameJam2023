using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

namespace GameFlow
{
    public class LevelChanger : MonoBehaviour
    {
        [Header("Levels must be in right order")]
        [SerializeField] private List<Level> _levels;
        [SerializeField] private Player.Player _player;

        private Level _currentLevel;
        private int _currentLevelIndex;

        private void Start()
        {
            EventBusSingleton.Instance.Subscribe<LevelComplete>(OnLevelComplete);
            _currentLevel = Instantiate(_levels[_currentLevelIndex]);
            PathfinderHandler.Instance.BakePath();
        }

        private void OnDestroy()
        {
            EventBusSingleton.Instance.Unsubscribe<LevelComplete>(OnLevelComplete);
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
    }
}