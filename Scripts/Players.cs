using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace WorldQuest
{
    public class Players : NetworkBehaviour
    {
        private HashSet<Player> players = new HashSet<Player>();

        [Server]
        public void Register(Tier tier, Player player)
        {
            if (players.Contains(player))
            {
                Debug.LogFormat("Player '{0}' is already registered to world quest '{1}'", player.name, transform.parent.name);
                return;
            }
            Debug.LogFormat("Registering player '{0}' to world quest '{1}'", player.name, transform.parent.name);
            players.Add(player);
            SetupPlayer(tier, player);
        }

        [Server]
        public void Unregister(Tier tier, Player player)
        {
            if (!players.Contains(player))
            {
                Debug.LogFormat("Player '{0}' is not registered to world quest '{1}'", player.name, transform.parent.name);
                return;
            }
            Debug.LogFormat("Unregistering player '{0}' to world quest '{1}'", player.name, transform.parent.name);
            players.Remove(player);
            TearDownPlayer(tier, player);
        }

        [Server]
        public void SetupTier(Tier tier)
        {
            foreach (var player in players)
            {
                SetupPlayer(tier, player);
            }
        }

        [Server]
        public void TearDownTier(Tier tier)
        {
            foreach (var player in players)
            {
                TearDownPlayer(tier, player);
            }
        }

        private void SetupPlayer(Tier tier, Player player)
        {
            Debug.LogFormat("Setup player '{0}'", player.name);
            tier.Register(player);
        }

        private void TearDownPlayer(Tier tier, Player player)
        {
            Debug.LogFormat("Tear down player '{0}'", player.name);
            tier.Unregister(player);
        }
    }
}