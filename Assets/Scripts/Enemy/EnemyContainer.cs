using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyContainer : MonoBehaviour
    {
        [field: SerializeField] public EnemyPhysics EnemyPhysics;
        [field: SerializeField] public EnemyView EnemyView;
        [field: SerializeField] public EnemyStateMachine StateMachine;
    }
}