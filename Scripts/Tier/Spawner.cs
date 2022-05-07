using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace WorldQuest
{
    [Serializable] public class UnityEventGameObject : UnityEvent<GameObject>
    {
    }
    
    [RequireComponent(typeof(Tier))]
    public class Spawner: NetworkBehaviour
    {
        public InstanceSpawnPoint[] spawnPoints;
        
        public UnityEventGameObject onSpawn;
        
        public UnityEventGameObject onDestroy;
        
        private GameObject[] _spawnedGameObjects;

        private void Reset()
        {
            // populating with child spawn point
            spawnPoints = GetComponentsInChildren<InstanceSpawnPoint>();
        }

        private void Awake()
        {
            _spawnedGameObjects = new GameObject[spawnPoints.Length];
        }

        public override void OnStartServer()
        {
            var tier = GetComponent<Tier>();
            tier.onSetup.AddListener(Setup);
            tier.onTearDown.AddListener(TearDown);
        }

        [Server]
        public void Setup()
        {
            Debug.LogFormat("Setting up spawns for tier '{0}'", name);

            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (_spawnedGameObjects[i] != null)
                {
                    continue;
                }
                
                var spawnPoint = spawnPoints[i];
                var spawnPointTransform = spawnPoint.transform;
                GameObject spawned = Instantiate(spawnPoint.prefab.gameObject, spawnPointTransform.position,
                    spawnPointTransform.rotation);
                _spawnedGameObjects[i] = spawned;
                spawned.name = spawnPoint.prefab.name;
                NetworkServer.Spawn(spawned);
                
                onSpawn.Invoke(spawned);
            }
        }

        [Server]
        public void TearDown()
        {
            for (int i = 0; i < _spawnedGameObjects.Length; i++)
            {
                if (_spawnedGameObjects[i] == null)
                {
                    continue;
                }
                
                onDestroy.Invoke(_spawnedGameObjects[i]);
                NetworkServer.Destroy(_spawnedGameObjects[i]);
                _spawnedGameObjects[i] = null;
            }

            Debug.LogFormat("Tearing down spawn for tier '{0}'", name);
        }
    }
}