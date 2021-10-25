using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace WorldQuest
{
    [RequireComponent(typeof(Players))]
    public abstract class Involvement : NetworkBehaviour
    {
        public float rate = 1f;

        public readonly Dictionary<string, float> scores = new Dictionary<string, float>();

        private Players _players;

        [ServerCallback]
        private void Start()
        {
            _players = GetComponent<Players>();
        }

        [Server]
        public virtual void OnRestart()
        {
            if (Debug.isDebugBuild)
            {
                foreach (var nameScorePair in scores)
                {
                    Debug.LogFormat("\n{0} - player: {1}\tscore: {2}", GetType(), nameScorePair.Key,
                        nameScorePair.Value);
                }
            }

            scores.Clear();

            foreach (var player in _players.players)
            {
                OnPlayerEnter(player);
            }
        }

        [Server]
        public virtual void OnPlayerEnter(Player player)
        {
            scores[player.name] = 0;
        }

        [Server]
        protected void Add(Entity entity, float score)
        {
            var totalScore = scores[entity.name] + score * rate;
            Debug.LogFormat("{0} scored {1} for a total of {2}", entity.name, score, totalScore);
            scores[entity.name] = totalScore;
        }
    }
}