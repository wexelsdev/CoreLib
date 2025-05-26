using UnityEngine;

namespace CoreLib.PrimitiveEffects
{
    public class SparksMaker
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public float Spread { get; set; } = 15f;
        public float ForceMin { get; set; } = 3f;
        public float ForceMax { get; set; } = 4.8f;
        public Color Color { get; set; } = Color.black;
    
        public void Play(Vector3 position, Vector3 direction)
        {
            SparkEffects.SparkEffects.Create(Random.Range(Min, Max), position, direction, Color, Spread, ForceMin, ForceMax);
        }
    }
}