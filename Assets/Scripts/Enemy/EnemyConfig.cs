using UnityEngine;

public class EnemyConfig : ScriptableObject
{
    [field: SerializeField] public int Health { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; }
}
