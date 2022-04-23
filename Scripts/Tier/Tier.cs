﻿using Mirror;
using UnityEngine;

namespace WorldQuest
{
    public class Tier : NetworkBehaviour
    {
        [SerializeField]
        private string _description;

        public InstanceSpawnPoint[] spawnPoints;

        public Goal[] goals;

        [HideInInspector]
        public GameObject[] spawnedGameObjects;

        [HideInInspector]
        public bool active;

        public string Description
        {
            get
            {
                var desc = _description + "\n";
                foreach (var goal in goals)
                {
                    desc = desc + "\n" + goal.Description;
                }

                return desc;
            }
        }

        [Server]
        public void Setup()
        {
            Debug.LogFormat("Setting up tier '{0}'", name);
            
            foreach (var goal in goals)
            {
                goal.Setup();
            }

            if (spawnedGameObjects == null || spawnedGameObjects.Length != spawnPoints.Length)
            {
                spawnedGameObjects = new GameObject[spawnPoints.Length];
            }

            for (int i = 0; i < spawnPoints.Length; i++)
            {
                var spawnPoint = spawnPoints[i];
                var spawnPointTransform = spawnPoint.transform;
                GameObject spawned = Instantiate(spawnPoint.prefab.gameObject, spawnPointTransform.position,
                    spawnPointTransform.rotation);
                spawnedGameObjects[i] = spawned;
                spawned.name = spawnPoint.prefab.name;
                NetworkServer.Spawn(spawned);
            }

            active = true;
        }

        [Server]
        public void TearDown()
        {
            active = false;

            foreach (var goal in goals)
            {
                goal.TearDown();
            }
            
            for (int i = 0; i < spawnedGameObjects.Length; i++)
            {
                NetworkServer.Destroy(spawnedGameObjects[i]);
                spawnedGameObjects[i] = null;
            }
            
            Debug.LogFormat("Tearing down tier '{0}'", name);
        }

        private void OnValidate()
        {
            spawnPoints = GetComponentsInChildren<InstanceSpawnPoint>();

            if (goals == null || goals.Length == 0)
            {
                Debug.LogWarningFormat("There are no goals for Tier '{0}'", name);
            }
        }

        public bool IsFulfilled()
        {
            var fulfilled = true;
            foreach (var goal in goals)
            {
                fulfilled &= goal.IsFulfilled();
            }

            return fulfilled;
        }

        [Server]
        public void Register(Player player)
        {
            foreach (var goal in goals)
            {
                goal.RegisterPlayer(player);
            }
        }

        [Server]
        public void Unregister(Player player)
        {
            foreach (var goal in goals)
            {
                goal.UnregisterPlayer(player);
            }
        }
    }
}