using UnityEngine;
using Random = UnityEngine.Random;

namespace Normal
{
    public class WaypointProvider : Singleton<WaypointProvider>
    {
        [SerializeField] private int waypointCount = 300;
        [SerializeField] private Transform sceneCorner0, sceneCorner1;
        
        private Vector3[] _waypoints;
        
        public Vector3 GetRandomWaypoint()
        {
            return _waypoints[Random.Range(0, _waypoints.Length)];
        }

        private void Start()
        {
            _waypoints = new Vector3[waypointCount];
            for (int i = 0; i < waypointCount; i++)
            {
                _waypoints[i] = new Vector3(
                    Random.Range(sceneCorner0.position.x, sceneCorner1.position.x),
                    0,
                    Random.Range(sceneCorner0.position.z, sceneCorner1.position.z)
                );
            }
        }
    }
}