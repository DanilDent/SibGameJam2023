using UnityEngine;
using Pathfinding;

namespace Enemy
{
    public class PathfinderHandler : MonoBehaviour
    {
        public static PathfinderHandler Instance { get; private set; }

        [SerializeField] private AstarPath _pathfinder;
        private NavGraph _graph;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                transform.parent = null;
                _graph = _pathfinder.graphs[0];
            }
            else
            {
                Instance = null;
            }
        }

        public void BakePath()
        {
            //_pathfinder.gra
            AstarPath.active.Scan(); 
        }
    }
}