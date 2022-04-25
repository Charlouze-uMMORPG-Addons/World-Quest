using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace WorldQuest.Goals
{
    public class KillGoal : ProgressGoal
    {
        public string targetName;
        public Monster killTarget;
        public int killNeeded;
        public Players players;

        private Dictionary<string, UnityAction<Entity>> onKilledListeners =
            new Dictionary<string, UnityAction<Entity>>();

        public void Reset()
        {
            // Populating with Players on parent GO
            players = transform.parent.GetComponent<Players>();
        }

        public override void OnStartServer()
        {
            players.onPlayerEnter.AddListener(RegisterPlayer);
            players.onPlayerLeave.AddListener(UnregisterPlayer);
        }

        private void RegisterPlayer(Player player)
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

        private void UnregisterPlayer(Player player)
        {
            if (onKilledListeners.ContainsKey(player.name))
            {
                var combat = player.GetComponent<Combat>();
                combat.onKilledEnemy.RemoveListener(onKilledListeners[player.name]);
                onKilledListeners.Remove(player.name);
            }
        }

        protected override int EndProgress => killNeeded;

        public override string Description => $"{progress}/{killNeeded} {targetName} killed";
    }
}