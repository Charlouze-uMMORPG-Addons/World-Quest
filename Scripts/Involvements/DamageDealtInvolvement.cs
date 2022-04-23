using System.Collections.Generic;
using Mirror;
using UnityEngine.Events;

namespace WorldQuest.Involvements
{
    public class DamageDealtInvolvement : Involvement
    {
        private Dictionary<int, UnityAction<Entity, int>> onRecvDamagelisteners =
            new Dictionary<int, UnityAction<Entity, int>>();

        public override void OnStartServer()
        {
            base.OnStartServer();
            _tierManager.onTierSetup.AddListener(OnTierSetup);
            _tierManager.onTierTearDown.AddListener(OnTierTearDown);
        }

        [Server]
        public void OnTierSetup(Tier tier)
        {
            var spawner = tier.GetComponent<Spawner>();
            
            if (spawner == null) return;
            
            for (int i = 0; i < spawner.spawnedGameObjects.Length; i++)
            {
                onRecvDamagelisteners[i] = (entity, damage) =>
                {
                    if (scores.ContainsKey(entity.name))
                    {
                        Add(entity, damage * rate);
                    }
                };
                spawner.spawnedGameObjects[i].GetComponent<Combat>().onServerReceivedDamage
                    .AddListener(onRecvDamagelisteners[i]);
            }
        }

        [Server]
        public void OnTierTearDown(Tier tier)
        {
            var spawner = tier.GetComponent<Spawner>();
            
            if (spawner == null) return;

            for (int i = 0; i < spawner.spawnedGameObjects.Length; i++)
            {
                spawner.spawnedGameObjects[i].GetComponent<Combat>().onServerReceivedDamage
                    .RemoveListener(onRecvDamagelisteners[i]);
            }

            onRecvDamagelisteners.Clear();
        }
    }
}