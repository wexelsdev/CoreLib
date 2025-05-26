using System;
using System.Reflection;
using AdminToys;
using CoreLib.DevTools.Other;
using Exiled.API.Features.Toys;
using MEC;
using UnityEngine;

namespace CoreLib.DevTools.Pooling
{
    public class PooledPrimitive : IDisposable
    {
        private readonly Primitive _primitive;
        private readonly PrimitivePool _pool;

        internal PooledPrimitive(PrimitiveObjectToy adminToyBase, PrimitivePool pool)
        {
            _primitive = CreatePrimitive(adminToyBase);
            _pool = pool;
        }

        private static Primitive CreatePrimitive(PrimitiveObjectToy adminToyBase)
        {
            var constructor = typeof(Primitive).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] { typeof(PrimitiveObjectToy) },
                null
            ) ?? throw new MissingMethodException("Конструктор Primitive не найден.");

            return (Primitive)constructor.Invoke(new object[] { adminToyBase });
        }

        public void Dispose()
        {
            _pool.Return(this);
        }

        public PrimitiveType Type
        {
            get => _primitive.Type;
            set => _primitive.Type = value;
        }
    
        public Vector3 Position
        {
            get => _primitive.Position;
            set => _primitive.Position = value;
        }
    
        public Vector3 Scale
        {
            get => _primitive.Scale;
            set => _primitive.Scale = value;
        }
    
        public Color Color
        {
            get => _primitive.Color;
            set => _primitive.Color = value;
        }
    
        public Quaternion Rotation
        {
            get => _primitive.Rotation;
            set => _primitive.Rotation = value;
        }
    
        public PrimitiveFlags Flags
        {
            get => _primitive.Flags;
            set => _primitive.Flags = value;
        }
    
        public PrimitiveObjectToy Base => _primitive.Base;
    
        public bool IsStatic
        {
            get => _primitive.IsStatic;
            set => _primitive.IsStatic = value;
        }
    
        public byte MovementSmoothing
        {
            get => _primitive.MovementSmoothing;
            set => _primitive.MovementSmoothing = value;
        }

        public Primitive Primitive => _primitive;

        public void DestroyDelayed(float delay)
        {
            Timing.CallDelayed(delay, Kill);
        }
    
        internal void SetProperties(PrimitiveType type, Vector3 position, Vector3 scale, Color color, Quaternion rotation, PrimitiveFlags flags)
        {
            Flags = flags;
            Type = type;
            Position = position;
            Scale = scale;
            Color = Prefabs.ToGlowColor(color);
            Rotation = rotation;
        }
    
        internal void Kill()
        {
            Dispose();
        }
    }
}