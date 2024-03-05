using System;
using UnityEngine;

namespace Normal
{
    public enum CubeType
    {
        A,
        B,
        C,
        D
    }
    
    [RequireComponent(typeof(Rigidbody))]
    public class MovingCube : MonoBehaviour
    {
        public CubeType CubeType
        {
            get => cubeType;
            set => cubeType = value;
        }

        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private CubeType cubeType;
        [SerializeField] private float noCollisionBaseTime = 1f;
        [SerializeField] private float noCollisionTimeIncrease = 0.005f;

        private Vector3 _target;
        private float _noCollisionTimer;
        private MeshRenderer _meshRenderer;
        private Rigidbody _rigidbody;
        private bool _cannotCollide;
        private Color _originalColor;

        public void Initialize(CubeType type, Vector3 position)
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _rigidbody = GetComponent<Rigidbody>();

            transform.position = position;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.velocity = Vector3.zero;
            
            cubeType = type;
            _noCollisionTimer = noCollisionBaseTime + CubePool.instance.ActiveCount * noCollisionTimeIncrease;
            _cannotCollide = true;
            
            // set data from asset
            var movingCubeAsset = Resources.Load<MovingCubeAsset>($"MovingCubeAsset{cubeType}");
            moveSpeed = movingCubeAsset.moveSpeed;
            transform.localScale = Vector3.one * movingCubeAsset.scale;
            
            _target = WaypointProvider.instance.GetRandomWaypoint();
            transform.LookAt(_target, Vector3.up);

            _originalColor = movingCubeAsset.color;
            _meshRenderer.material.color = Color.red;
        }
        
        private void Update()
        {
            // move towards target
            transform.Translate(moveSpeed * Time.deltaTime * Vector3.forward);
            if (Vector3.SqrMagnitude(transform.position - _target) < 0.1f)
            {
                _target = WaypointProvider.instance.GetRandomWaypoint();
                transform.LookAt(_target, Vector3.up);
            }
            
            // safe collision time
            if (_cannotCollide)
            {
                _noCollisionTimer -= Time.deltaTime;
                
                if (_cannotCollide && _noCollisionTimer <= 0)
                {
                    _cannotCollide = false;
                    _meshRenderer.material.color = _originalColor;
                }
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.TryGetComponent(out MovingCube otherCube) && _noCollisionTimer <= 0)
            {
                if (otherCube.CubeType == cubeType)
                {
                    // same type collision, spawn a third one
                    // determine a primary one to handle spawning
                    if (GetInstanceID() < otherCube.GetInstanceID())
                    {
                        // trigger safe collision time
                        _noCollisionTimer = noCollisionBaseTime + CubePool.instance.ActiveCount * noCollisionTimeIncrease;
                        _cannotCollide = true;
                        _meshRenderer.material.color = Color.red;
                        
                        var spawnedObject = CubePool.instance.GetCube();
                        if (spawnedObject == null) return;
                        
                        var position = (transform.position + other.transform.position) / 2;
                        spawnedObject.GetComponent<MovingCube>().Initialize(cubeType, position);
                    }
                }
                else
                {
                    // different type collision, destroy
                    CubePool.instance.ReturnCube(gameObject);
                }
            }
            
            // collide with wall, reflect direction
            if (other.gameObject.CompareTag("Wall"))
            {
                transform.forward = Vector3.Reflect(transform.forward, other.contacts[0].normal);
            }
        }
    }
}