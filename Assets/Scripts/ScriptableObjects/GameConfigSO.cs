using GameTime;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "New game config", menuName = "Config/Game config")]
    public class GameConfigSO : ScriptableObject
    {
        public GameTimeSO GameTime;
        public TrackSO[] Tracks;
        public PlayerSO Player;
    }
}
