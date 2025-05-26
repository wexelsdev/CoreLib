using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreLib.PrimitiveEffects.Other
{
    public class IntervalRecord
    {
        public IntervalRecord(float minIntervalSize) => _minIntervalSize = minIntervalSize;

        public IReadOnlyList<float> Points => _points;
    
        public bool Add(float value)
        {
            if (_points.Count > 0)
            {
                List<float> points = _points;
                float last = points[points.Count - 1];
                if (last > value)
                {
                    throw new ArgumentException(
                        $"Points should be added in ascending order. New point: {value}");
                }
                if (value - last < _minIntervalSize)
                {
                    _points[_points.Count - 1] = value;
                    return false;
                }
            }
            _points.Add(value);
            return true;
        }
    
        public void Clear()
        {
            _points.Clear();
        }

        public void Remove(float point)
        {
            if (!_points.Contains(point))
                return;
            _points.Remove(point);
        }

        public void Remove(Func<float, bool> condition)
        {
            List<float> localList = _points.Where(condition).ToList();
            foreach (float point in localList)
            {
                _points.Remove(point);
            }
        }

        public void RemoveAt(int index)
        {
            _points.RemoveAt(index);
        }
    
        public bool ContainsPoint(float value)
        {
            int index = _points.BinarySearch(value);
            if (index < 0)
            {
                index = ~index;
            }
            return index % 2 != 0;
        }
    
        private readonly List<float> _points = new();
        private readonly float _minIntervalSize;
    }
}