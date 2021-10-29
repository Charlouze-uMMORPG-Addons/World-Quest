using System.Collections.Generic;
using System.Text;
using Mirror;
using UnityEngine;

namespace WorldQuest
{
    public class Rewards : NetworkBehaviour
    {
        [SerializeField, TextArea(1, 30)] public string text;
        public long gold;
        public long experience;
        public Npc rewarder;

        private SyncDictionary<string, float> involvements = new SyncDictionary<string, float>();
        private SyncDictionary<string, int> ranking = new SyncDictionary<string, int>();

        private Involvement[] _involvements;

        [ServerCallback]
        private void Start()
        {
            _involvements = GetComponents<Involvement>();
        }

        [Server]
        public void Setup()
        {
            ComputeInvolments();
            ComputeRanking();
            rewarder.GetComponent<NpcRewards>().rewards = this;
            rewarder.Show();
        }

        [Server]
        public void TearDown()
        {
            involvements.Clear();
            ranking.Clear();
            rewarder.Hide();
        }

        private void ComputeInvolments()
        {
            foreach (var involvement in _involvements)
            {
                foreach (var nameScorePair in involvement.scores)
                {
                    if (!involvements.ContainsKey(nameScorePair.Key))
                    {
                        involvements[nameScorePair.Key] = 0;
                    }

                    involvements[nameScorePair.Key] += nameScorePair.Value;
                }
            }
        }

        private void ComputeRanking()
        {
            var involvementList = new List<KeyValuePair<string, float>>();
            foreach (var involvement in involvements)
            {
                involvementList.Add(involvement);
            }

            involvementList.Sort((involvement1, involvement2) => involvement2.Value.CompareTo(involvement1.Value));
            for (var i = 0; i > involvementList.Count; i++)
            {
                ranking[involvementList[i].Key] = i + 1;
            }
        }

        public string Text(Player player)
        {
            var txt = new StringBuilder(text);
            txt.Replace("{GOLD}", gold.ToString());
            txt.Replace("{EXP}", experience.ToString());
            txt.Replace("{RANK}", Rank(player).ToString());
            return txt.ToString();
        }

        private int Rank(Player player)
        {
            if (ranking.ContainsKey(player.name))
            {
                return ranking[player.name];
            }

            return ranking.Count + 1;
        }
    }
}