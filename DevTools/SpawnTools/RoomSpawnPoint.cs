using CoreLib.DevTools.SpawnTools.Models;
using Exiled.API.Enums;
using UnityEngine;

namespace CoreLib.DevTools.SpawnTools
{
    public class RoomSpawnPoint
    {
        private readonly bool _freeze;

        public RoomSpawnPoint(RoomType room, Vector3 offset, Vector3 rotation, int chance, bool freeze = false)
        {
            _freeze = freeze;
            Room = room;
            Offset = offset;
            Rotation = rotation;
            Chance = chance;
        }

        public RoomType Room { get; }
        public Vector3 Offset { get; }
        public Vector3 Rotation { get; }
        public int Chance { get; }
    
        public SpawnInfo Info
        {
            get
            {
                Vector3 position = GetPosition();
                Vector3 rotation = GetRotation().eulerAngles;
                return new SpawnInfo(position, rotation, _freeze);
            }
        }

        private Vector3 GetPosition()
        {
            Exiled.API.Features.Room room = Exiled.API.Features.Room.Get(Room);
            return room.Position + room.Transform.TransformDirection(Offset);
        }

        private Quaternion GetRotation()
        {
            Exiled.API.Features.Room room = Exiled.API.Features.Room.Get(Room);
            return room.Transform.rotation * Quaternion.Euler(Rotation);
        }
    }
}