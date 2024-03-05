using UnityEngine;

namespace Normal
{
    [CreateAssetMenu(fileName = "MovingCubeAsset", menuName = "MovingCubeAsset")]
    public class MovingCubeAsset : ScriptableObject
    {
        public float moveSpeed = 1f;
        public Color color;
        public float scale;
    }
}