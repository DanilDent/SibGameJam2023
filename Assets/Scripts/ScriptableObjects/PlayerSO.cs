using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "New player config", menuName = "Config/Player config")]
    public class PlayerSO : ScriptableObject
    {
        public float HitBeatEffectDuration = 0.3f;
        public float MovementSpeed;
        public float MovementForce;
        public float DashForce;
        public float AttackDashForce;
        public float AttackDashTimeScaleFactor;
        public float DashTimeScaleFactor;
        public float AttackSpeed;
        public float DashDelaySec;
        public int Damage;
        public int Health;

        private Player.Player _player;

        public void Init(Player.Player player)
        {
            _player = player;
        }

        public void OnValidate()
        {
            if (_player == null)
            {
                return;
            }

            _player.FillFromConfig(this);
        }
    }
}
