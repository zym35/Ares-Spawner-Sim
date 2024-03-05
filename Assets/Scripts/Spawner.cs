using System;
using UnityEngine;

namespace Normal
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private CubeType cubeType;
        [SerializeField] private float baseSpawnRate = 1f;
        [SerializeField] private float adjustmentMultiplier = 0.5f;
        [SerializeField] private Transform spawnLocation;
        
        private float _spawnTimer;
        private float _spawnRate;

        private void Awake()
        {
            _spawnRate = baseSpawnRate;
        }

        public void AdjustSpawnRate(float adjustment)
        {
            _spawnRate = baseSpawnRate + (adjustment - 0.5f) * adjustmentMultiplier;
            _spawnRate = Mathf.Clamp(_spawnRate, 0.1f, 10f);
        }

        private void Update()
        {
            _spawnTimer += Time.deltaTime;
            if (_spawnTimer > 1 / _spawnRate)
            {
                _spawnTimer = 0;
                Spawn();
            }
        }

        private void Spawn()
        {
            var cube = CubePool.instance.GetCube();
            if (cube == null) return;
            
            cube.GetComponent<MovingCube>().Initialize(cubeType, spawnLocation.position);
        }
    }
}