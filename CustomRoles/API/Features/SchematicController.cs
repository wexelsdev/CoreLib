using Exiled.API.Features;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using MEC;
using Mirror;
using UnityEngine;

namespace CoreLib.CustomRoles.API.Features
{
    public class SchematicController : MonoBehaviour
    {
        public string? schematicName;
    
        private SchematicObject? _schematicObject;
        private bool _isInvisible;
        private bool _isModelLocked;
        private Player _owner = null!;

        public void Init(string schematic)
        {
            _owner = Player.Get(gameObject);
        
            schematicName = schematic;
        
            SpawnPlayerModel();
        }

        private void Update()
        {
            if (_isModelLocked)
                return;

            _schematicObject!.Position = _owner.Position;
            _schematicObject.Rotation = _owner.Rotation;
        }
    
        public void TogglePlayerModel()
        {
            _isInvisible = !_isInvisible;

            if (_isInvisible)
                DestroyPlayerModel();
            else
                SpawnPlayerModel();
        }

        private void SpawnPlayerModel()
        {
            _isModelLocked = true;
            
            _schematicObject = ObjectSpawner.SpawnSchematic(schematicName!, _owner.Position, _owner.Rotation, Vector3.one);

            Timing.CallDelayed(0.1f, () => _isModelLocked = false);

            foreach (NetworkIdentity identity in _schematicObject.NetworkIdentities)
                _owner.NetworkIdentity.connectionToClient.Send(new ObjectDestroyMessage {netId = identity.netId});
        }

        private void DestroyPlayerModel()
        {
            if (_schematicObject == null)
                return;
        
            _schematicObject.Destroy();

            _schematicObject = null;
        }
    }
}