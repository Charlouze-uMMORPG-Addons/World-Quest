using System.Collections.Generic;
using Mirror;
using UnityEngine;
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
            for (int i = 0; i < tier.spawnedGameObjects.Length; i++)
            {
                onRecvDamagelisteners[i] = (entity, damage) =>
                {
                    if (scores.ContainsKey(entity.name))
                    {
                        Add(entity,damage * rate);
                    }
                };
                tier.spawnedGameObjects[i].GetComponent<Combat>().onServerReceivedDamage
                    .AddListener(onRecvDamagelisteners[i]);
            }
        }

        [Server]
        public void OnTierTearDown(Tier tier)
        {
            for (int i = 0; i < tier.spawnedGameObjects.Length; i++)
            {
                tier.spawnedGameObjects[i].GetComponent<Combat>().onServerReceivedDamage
                    .RemoveListener(onRecvDamagelisteners[i]);
            }

            onRecvDamagelisteners.Clear();
        }
    }
}