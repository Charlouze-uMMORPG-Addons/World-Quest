using System.Collections.Generic;
using UnityEngine.Events;

namespace WorldQuest.Involvements
{
    public class DamageDealtInvolvement : Involvement
    {
        public float rate = 1f;

        private Dictionary<int, UnityAction<Entity, int>> onRecvDamagelisteners =
            new Dictionary<int, UnityAction<Entity, int>>();

        public void OnTierSetup(Tier tier)
        {
            for (int i = 0; i < tier.spawnedGameObjects.Length; i++)
            {
                onRecvDamagelisteners[i] = (entity, damage) =>
                {
                    if (scores.ContainsKey(entity.name))
                    {
                        scores[entity.name] += damage * this.rate;
                    }
                };
                tier.spawnedGameObjects[i].GetComponent<Combat>().onServerReceivedDamage
                    .AddListener(onRecvDamagelisteners[i]);
            }
        }

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