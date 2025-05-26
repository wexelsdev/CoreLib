using CoreLib.DevTools.Other;
using CoreLib.PrimitiveEffects.Other;
using UnityEngine;

namespace CoreLib.PrimitiveEffects
{
    public abstract class PrimitiveEffect
    {
        public virtual Penetrate? Penetrate { get; } = null;
        public virtual bool IsPenetrating { get; } = false;
        public virtual SparksMaker? Sparks { get; } = null;
        public abstract float MaxDistance { get; set; }

        protected virtual void Effect(Vector3 from, Vector3 to, IntervalRecord? penetrationRecord, bool debugLine = false)
        {
            if (debugLine) Prefabs.RentLine(from, to, new Color(1f, 1f, 1.4f) * 50);
        }

        protected virtual void Effect(Vector3 position, bool debug = false)
        {
            if (debug) Prefabs.RentPrimitive(PrimitiveType.Cube, position, Vector3.one, new Color(1f, 1f, 1.4f) * 50);
        }
    
        public void Play(Vector3 origin, Vector3 direction)
        {
            direction.Normalize();
            EffectProcessor unused = new EffectProcessor(this, origin, direction);
        }

        public void Play(Vector3 position)
        {
            Effect(position);
        }

        public void Play(Vector3 from, Vector3 to, IntervalRecord? penetrationRecord)
        {
            Effect(from, to, penetrationRecord);
        }

        public virtual void Hit(Ray ray, RaycastHit hit)
        {
            Sparks?.Play(hit.point, -ray.direction);
        }
    }
}