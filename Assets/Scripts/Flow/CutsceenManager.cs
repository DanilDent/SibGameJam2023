using Enemy;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace GameFlow
{
    public class CutsceenManager : MonoBehaviour
    {
        [SerializeField] private List<Cutsceen> _cutsceens;
        private int _cutsceenIndex;
        private Player.Player _player;

        private IEnumerator Start()
        {
            _player = FindFirstObjectByType<Player.Player>();
            yield return new WaitForSeconds(.001f);
            _cutsceenIndex = 0;
            StartCutscene();
            EventBusSingleton.Instance.Subscribe<LevelLoaded>(OnLevelComplete);    
        }

        private void OnDestroy()
        {
            EventBusSingleton.Instance.Unsubscribe<LevelLoaded>(OnLevelComplete);
        }

        private void StartCutscene()
        {
            Level firstLevel = FindObjectOfType<Level>();

            StartCoroutine(_cutsceens[_cutsceenIndex].StartCutsceen(firstLevel._levelSpawner, _player));
            _cutsceenIndex++;
        }

        private void OnLevelComplete(LevelLoaded signal)
        {
            if (_cutsceenIndex >= _cutsceens.Count)
                return;

            StartCoroutine(_cutsceens[_cutsceenIndex].StartCutsceen(signal.Level._levelSpawner, _player));
            _cutsceenIndex++;
        }
    }
}