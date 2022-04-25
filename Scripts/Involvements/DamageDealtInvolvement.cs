using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace WorldQuest.Involvements
{
    public class DamageDealtInvolvement : Involvement
    {
        public Spawner[] spawners;
        
        private Dictionary<uint, UnityAction<Entity, int>> onRecvDamagelisteners =
            new Dictionary<uint, UnityAction<Entity, int>>();

        private void Reset()
        {
            // populating with spawners on children
            spawners = GetComponentsInChildren<Spawner>();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            foreach (var spawner in spawners)
            {
                spawner.onSpawn.AddListener(OnSpawnGameObject);
                spawner.onDestroy.AddListener(OnDestroyGameObject);
            }
        }

        [Server]
        private void OnSpawnGameObject(GameObject gameObject)
        {
            var combat = gameObject.GetComponent<Combat>();
            if (combat != null)
            {
                onRecvDamagelisteners[combat.netId] = (entity, damage) =>
                {
                    if (scores.ContainsKey(entity.name))
                    {
                        Add(entity, damage * rate);
                    }
                };
                combat.onServerReceivedDamage.AddListener(onRecvDamagelisteners[combat.netId]);
            }
        }

        [Server]
        private void OnDestroyGameObject(GameObject gameObject)
        {
            var combat = gameObject.GetComponent<Combat>();
            if (combat != null)
            {
                combat.onServerReceivedDamage.RemoveListener(onRecvDamagelisteners[combat.netId]);
                onRecvDamagelisteners.Remove(combat.netId);
            }
        }
    }
}