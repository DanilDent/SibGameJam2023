using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "New player config", menuName = "Config/Player config")]
    public class PlayerSO : ScriptableObject
    {
        public float MovementSpeed;
        public float MovementForce;
        public float DashForce;
        public float AttackDashForce;
        public float AttackDashTimeScaleFactor;
        public float DashTimeScaleFactor;
        public float AttackSpeed;
        public float Damage;
    }
}
