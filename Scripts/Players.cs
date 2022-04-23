using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace WorldQuest
{
    public class Players : NetworkBehaviour
    {
        public UnityEventPlayer onPlayerEnter = new UnityEventPlayer();

        public UnityEventPlayer onPlayerLeave = new UnityEventPlayer();

        public readonly HashSet<Player> players = new HashSet<Player>();

        [Server]
        public void Register(Player player)
        {
            if (players.Contains(player))
            {
                Debug.LogFormat("Player '{0}' is already registered to world quest '{1}'", player.name, transform.name);
                return;
            }

            Debug.LogFormat("Registering player '{0}' to world quest '{1}'", player.name, transform.name);
            players.Add(player);
            onPlayerEnter.Invoke(player);
            player.GetComponent<PlayerWorldQuests>().Add(gameObject);
        }

        [Server]
        public void Unregister(Player player)
        {
            if (!players.Contains(player))
            {
                Debug.LogFormat("Player '{0}' is not registered to world quest '{1}'", player.name, transform.name);
                return;
            }

            Debug.LogFormat("Unregistering player '{0}' to world quest '{1}'", player.name, transform.name);
            players.Remove(player);
            onPlayerLeave.Invoke(player);
            player.GetComponent<PlayerWorldQuests>().Remove(gameObject);
        }

        public void ForEach(Action<Player> action)
        {
            foreach (var player in players)
            {
                action(player);
            }
        }
    }
}