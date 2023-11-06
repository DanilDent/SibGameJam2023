using Enemy;
using System.Collections.Generic;
using UnityEngine;

namespace GameFlow
{
    public class CutsceenManager : MonoBehaviour
    {
        [SerializeField] private List<Cutsceen> _cutsceens;
        private int _cutsceenIndex;

        private void Start()
        {
            _cutsceenIndex = 0;
            EventBusSingleton.Instance.Subscribe<LevelComplete>(OnLevelComplete);    
        }

        private void OnDestroy()
        {
            EventBusSingleton.Instance.Unsubscribe<LevelComplete>(OnLevelComplete);
        }

        private void OnLevelComplete(LevelComplete signal)
        {
            if (_cutsceenIndex >= _cutsceens.Count)
                return;

            StartCoroutine(_cutsceens[_cutsceenIndex].StartCutsceen(signal.Level._levelSpawner));
            _cutsceenIndex++;
        }
    }
}