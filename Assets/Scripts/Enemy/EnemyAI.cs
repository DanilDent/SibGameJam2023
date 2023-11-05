using Pathfinding;
using UnityEngine;

namespace Enemy
{
    public class EnemyAI : MonoBehaviour
    {
        public Vector3 LastForce { get; private set; }

        private Transform _playerPos;
        private Path _path;
        private int _curretnWaypoint;
        public bool ReachedEndOfPath { get; private set; }
        private float _nextWaypointDistance = 1;
        private Seeker _seeker;
        private Rigidbody2D _rb;
        private float _speed;

        public void Init(Seeker seeker, Rigidbody2D rb, Transform playerPos, float speed)
        {
            _seeker = seeker;
            _rb = rb;
            _playerPos = playerPos;
            _speed = speed;

            InvokeRepeating("UpdatePath", 0f, .25f);
        }

        public void UpdatePath()
        {
            if (_seeker.IsDone())
            {
                _seeker.StartPath(_rb.position, _playerPos.position, OnPathComplete);
            }
        }

        public void UpdatePos()
        {
            if (_path == null)
                return;

            ReachedEndOfPath = _curretnWaypoint >= _path.vectorPath.Count;

            if (ReachedEndOfPath)
                return;

            Vector2 direction = ((Vector2)_path.vectorPath[_curretnWaypoint] - _rb.position).normalized;
            Vector3 force = _speed * Time.deltaTime * direction;
            _rb.AddForce(force);
            LastForce = force;
            float distance = Vector2.Distance(_rb.position, _path.vectorPath[_curretnWaypoint]);
            
            if (distance < _nextWaypointDistance)
                _curretnWaypoint++;
        }

        private void OnPathComplete(Path path)
        {
            if (!path.error)
            {
                _path = path;
                _curretnWaypoint = 0;
            }
        }
    }
}