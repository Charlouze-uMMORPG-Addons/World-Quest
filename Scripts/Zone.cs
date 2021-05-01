using Mirror;
using UnityEngine;

namespace WorldQuest
{
    [RequireComponent(typeof(Manager))]
    [RequireComponent(typeof(Players))]
    [RequireComponent(typeof(BoxCollider))]
    public class Zone : NetworkBehaviour
    {
        private Manager _manager;
        private Players _players;

        private void Awake()
        {
            _manager = GetComponent<Manager>();
            _players = GetComponent<Players>();
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponentInParent<Player>();
            if (player != null)
            {
                _players.Register(_manager.CurrentTier, player);
            }
            else
            {
                Debug.LogFormat("No player found for '{0}'", other.name);
            }
        }

        [ServerCallback]
        private void OnTriggerExit(Collider other)
        {
            var player = other.GetComponentInParent<Player>();
            if (player != null)
            {
                _players.Unregister(_manager.CurrentTier, player);
            }
            else
            {
                Debug.LogFormat("No player found for '{0}'", other.name);
            }
        }

        private void OnValidate()
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }

        // draw the world quest zone all the time to clearly see where it is
        private static readonly Color GIZMO_COLOR = new Color(1, 0.6f, 0.3f, 0.25f);
        private static readonly Color GIZMO_WIRE_COLOR = new Color(1, 1, 1, 0.8f);

        void OnDrawGizmos()
        {
            BoxCollider collider = GetComponent<BoxCollider>();

            // we need to set the gizmo matrix for proper scale & rotation
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.color = GIZMO_COLOR;
            Gizmos.DrawCube(collider.center, collider.size);
            Gizmos.color = GIZMO_WIRE_COLOR;
            Gizmos.DrawWireCube(collider.center, collider.size);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}