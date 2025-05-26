using UnityEngine;

namespace CoreLib.DevTools.DataTypes
{
    public class SecondCounter
    {
        public float TimeRunning
        {
            get => Time.time - _startTime;
            set => _startTime = Time.time - value;
        }
    
        public void Reset()
        {
            _startTime = Time.time;
        }
    
        public static implicit operator float(SecondCounter sc)
        {
            return sc.TimeRunning;
        }
    
        public static bool operator <(SecondCounter sc, float value)
        {
            return sc.TimeRunning < value;
        }
    
        public static bool operator >(SecondCounter sc, float value)
        {
            return sc.TimeRunning > value;
        }
    
        public static bool operator <=(SecondCounter sc, float value)
        {
            return sc.TimeRunning <= value;
        }
    
        public static bool operator >=(SecondCounter sc, float value)
        {
            return sc.TimeRunning >= value;
        }
    
        private float _startTime = Time.time;
    }
}