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
    }
}
