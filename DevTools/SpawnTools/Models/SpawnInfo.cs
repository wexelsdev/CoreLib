using UnityEngine;

namespace CoreLib.DevTools.SpawnTools.Models
{
    public class SpawnInfo
    {
        public SpawnInfo(Vector3 position, Vector3 rotation, bool isKinematic = true)
        {
            Position = position;
            Rotation = rotation;
            IsKinematic = isKinematic;
        }

        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public bool IsKinematic { get; set; }
    }
}