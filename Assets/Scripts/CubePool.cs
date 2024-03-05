using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Normal
{
    /// object pool for cubes
    public class CubePool : Singleton<CubePool>
    {
        public int ActiveCount { get; private set; }
        
        [SerializeField] private GameObject cubePrefab;
        [SerializeField] private int initialPoolSize = 2;
        [SerializeField] private int maxPoolSize = 10000;
        [SerializeField] private Text activeCountText;

        private List<GameObject> _pool;

        public GameObject GetCube()
        {
            if (ActiveCount == maxPoolSize)
            {
                return null;
            }
            
            foreach (GameObject t in _pool)
            {
                if (!t.activeInHierarchy)
                {
                    t.SetActive(true);
                    ActiveCount++;
                    return t;
                }
            }
            
            // if no inactive cubes, resize the pool
            Resize();
            
            var cube = _pool[^1];
            cube.SetActive(true);
            ActiveCount++;
            return cube;
        }
        
        public void ReturnCube(GameObject cube)
        {
            ActiveCount--;
            cube.SetActive(false);
        }
        
        private void Update()
        {
            activeCountText.text = $"Active count: {ActiveCount}";
        }

        private void Start()
        {
            _pool = new List<GameObject>(initialPoolSize);
            for (int i = 0; i < initialPoolSize; i++)
            {
                var cube = Instantiate(cubePrefab);
                cube.SetActive(false);
                _pool.Add(cube);
            }
        }

        private void Resize()
        {
            var addSize = _pool.Capacity;
            if (addSize * 2 >= maxPoolSize)
            {
                addSize = maxPoolSize - _pool.Capacity;
            }
            
            for (int i = 0; i < addSize; i++)
            {
                var cube = Instantiate(cubePrefab);
                cube.SetActive(false);
                _pool.Add(cube);
            }
        }
    }
}