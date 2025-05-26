using CoreLib.PrimitiveEffects.Other;
using UnityEngine;

namespace CoreLib.PrimitiveEffects
{
    public class EffectProcessor
    {
        public EffectProcessor(PrimitiveEffect effect, Vector3 origin, Vector3 direction)
        {
            _effect = effect;
            _maxDistance = effect.MaxDistance;
            if (effect is { IsPenetrating: true, Penetrate: not null })
                _maxPenetrationDistance = effect.Penetrate.MaxPenetrationDistance;
            _direction = direction;
            _position = origin;
            Vector3 startPosition = _position;
            ProcessEffect(origin, direction);
            Vector3 endPosition = _position;
            IntervalRecord? penetrationRecord =
                effect is { IsPenetrating: true, Penetrate: not null } ? _penetrationRecord : null;
            effect.Play(startPosition, endPosition, penetrationRecord);
        }
    
        private void ProcessEffect(Vector3 origin, Vector3 direction)
        {
            if (float.IsNaN(origin.x) || direction.sqrMagnitude < 0.01f) 
                return;
        
            _penetrationRecord?.Clear();
        
            float distance = 0f;
            float totalPenetration = 0f;
            Ray ray = new(origin, direction);

            while (distance < _maxDistance)
            {
                float remainingDistance = _maxDistance - distance;
                if (!Physics.Raycast(ray, out var hit, remainingDistance, WallMask))
                {
                    _position = ray.GetPoint(remainingDistance);
                    return;
                }

                distance += hit.distance;
                _position = hit.point;
                _effect.Hit(ray, hit);

                if (!_effect.IsPenetrating)
                    return;

                _penetrationRecord?.Add(distance);

                if (!TryPenetrate(out var exitHit, out var exitRay, out var thickness))
                    return;

                totalPenetration += thickness;
                if (totalPenetration > _maxPenetrationDistance)
                    return;

                distance += thickness;
                _position = exitHit.point + direction;
                _penetrationRecord?.Add(distance);
                _effect.Hit(exitRay, exitHit);
                ray.origin = _position;
            }
        }
    
        private bool TryPenetrate(out RaycastHit exitHit, out Ray exitRay, out float thickness)
        {
            exitRay = new Ray(_position, -_direction);
            thickness = 0f;
            int timeout = 100;

            while (thickness < _maxPenetrationDistance && timeout-- > 0)
            {
                exitRay.origin = exitRay.GetPoint(-0.5f);
                if (Physics.Raycast(exitRay, out var hit, 0.5f, WallMask))
                {
                    exitHit = hit;
                    thickness += 0.5f - hit.distance;
                    return true;
                }
                thickness += 0.5f;
            }

            exitHit = default;
            _position = exitRay.origin;
            return false;
        }
    
        private readonly float _maxDistance;
    
        private readonly float _maxPenetrationDistance = 7f;
    
        private const int WallMask = 134496257;

        private readonly PrimitiveEffect _effect;

        private readonly IntervalRecord? _penetrationRecord = new(0.01f);
    
        private readonly Vector3 _direction;
    
        private Vector3 _position;
    }
}