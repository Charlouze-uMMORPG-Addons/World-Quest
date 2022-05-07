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

        private readonly Dictionary<string, UnityAction<Entity>> onKilledListeners = new();

        public override string Description => $"{_progress}/{killNeeded} {targetName} killed";

        protected override bool IsIncrementing => true;

        protected override int StartProgress => 0;

        protected override int EndProgress => killNeeded;

        public void Reset()
        {
            // Populating with Players on parent GO
            players = transform.parent.GetComponentInChildren<Players>();
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
                    if (victim.name == killTarget.name) UpdateProgress(_progress + 1);
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
    }
}