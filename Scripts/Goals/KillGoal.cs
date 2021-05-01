using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace WorldQuest.Goals
{
    public class KillGoal : Goal
    {
        public string targetName;
        public Monster killTarget;
        public int killNeeded;

        private Dictionary<string, UnityAction<Entity>> onKilledListeners =
            new Dictionary<string, UnityAction<Entity>>();

        public override bool IsFulfilled()
        {
            return progress >= killNeeded;
        }

        public override void RegisterPlayer(Player player)
        {
            if (!onKilledListeners.ContainsKey(player.name))
            {
                var combat = player.GetComponent<Combat>();
                var onKilledListener = new UnityAction<Entity>(victim =>
                {
                    if (victim.name == killTarget.name)
                    {
                        progress++;
                    }
                });
                onKilledListeners[player.name] = onKilledListener;
                combat.onKilledEnemy.AddListener(onKilledListener);
            }
        }

        public override void UnregisterPlayer(Player player)
        {
            if (onKilledListeners.ContainsKey(player.name))
            {
                var combat = player.GetComponent<Combat>();
                combat.onKilledEnemy.RemoveListener(onKilledListeners[player.name]);
                onKilledListeners.Remove(player.name);
            }
        }

        public override string Tooltip()
        {
            return $"{progress}/{killNeeded} {targetName} killed";
        }
    }
}