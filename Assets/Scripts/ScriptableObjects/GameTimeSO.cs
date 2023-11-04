using Game;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "New game time config", menuName = "Config/Game time config")]
    public class GameTimeSO : ScriptableObject
    {
        public int NumberOfSteps => _numberOfSteps;
        public float EpsSec => _epsSec;

        [SerializeField] private int _numberOfSteps = 4;
        [SerializeField] private float _epsSec = 1e-2f;

        private GameTime.GameTime _gt;
        public void Init(GameTime.GameTime gt)
        {
            _gt = gt;
        }

        public void OnValidate()
        {
            if (_gt == null || ConfigContainer.Instance == null)
                return;

            _gt.Fill(ConfigContainer.Instance.Value);
            _gt.Restart();
        }
    }
}
