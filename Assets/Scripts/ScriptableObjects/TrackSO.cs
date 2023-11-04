using Game;
using UnityEngine;

namespace GameTime
{

    [CreateAssetMenu(fileName = "New track config", menuName = "Config/Track config")]
    public class TrackSO : ScriptableObject
    {
        public AudioClip Clip => _clip;
        public float ClockFullTurnSec => _clockFullTurnSec;

        [SerializeField] private AudioClip _clip;
        [SerializeField] private float _clockFullTurnSec = 4f;

        private GameTime _gt;
        public void Init(GameTime gt)
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
