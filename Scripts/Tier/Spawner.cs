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
        public UnityEventGameObject onSpawn;
        
        public UnityEventGameObject onDestroy;
        
        [HideInInspector]
        public GameObject[] spawnedGameObjects;

        private InstanceSpawnPoint[] _spawnPoints;

        public override void OnStartServer()
        {
            var tier = GetComponent<Tier>();
            tier.onSetup.AddListener(Setup);
            tier.onTearDown.AddListener(TearDown);
            _spawnPoints = GetComponentsInChildren<InstanceSpawnPoint>();
        }

        [Server]
        public void Setup()
        {
            Debug.LogFormat("Setting up spawns for tier '{0}'", name);

            if (spawnedGameObjects == null || spawnedGameObjects.Length != _spawnPoints.Length)
            {
                spawnedGameObjects = new GameObject[_spawnPoints.Length];
            }

            for (int i = 0; i < _spawnPoints.Length; i++)
            {
                var spawnPoint = _spawnPoints[i];
                var spawnPointTransform = spawnPoint.transform;
                GameObject spawned = Instantiate(spawnPoint.prefab.gameObject, spawnPointTransform.position,
                    spawnPointTransform.rotation);
                spawnedGameObjects[i] = spawned;
                spawned.name = spawnPoint.prefab.name;
                NetworkServer.Spawn(spawned);
                
                onSpawn.Invoke(spawned);
            }
        }

        [Server]
        public void TearDown()
        {
            for (int i = 0; i < spawnedGameObjects.Length; i++)
            {
                onDestroy.Invoke(spawnedGameObjects[i]);
                NetworkServer.Destroy(spawnedGameObjects[i]);
                spawnedGameObjects[i] = null;
            }

            Debug.LogFormat("Tearing down spawn for tier '{0}'", name);
        }
    }
}