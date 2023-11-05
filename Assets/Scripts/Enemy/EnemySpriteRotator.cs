using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemySpriteRotator : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rb;
        private Transform _player;
        private bool _inited;

        public void Init(SpriteRenderer spite, Rigidbody2D rb, Transform player)
        {
            _spriteRenderer = spite;
            _rb = rb;
            _inited = true;
            _player = player;
        }

        private void Update()
        {
            if (!_inited)
                return;

            Vector3 rotVector = _player.transform.position - transform.position;
            _spriteRenderer.flipX = rotVector.x < 0;
        }
    }
}