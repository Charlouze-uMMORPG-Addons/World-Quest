using System.Collections.Generic;
using System.Text;
using Mirror;
using UnityEngine;

namespace WorldQuest.Goals
{
    public class RewardManager : NetworkBehaviour
    {
        public Tier rewardTier;
        public Spawner spawner;
        
        [SerializeField, TextArea(1, 30)] public string text;
        public long gold;
        public long experience;

        [HideInInspector]
        [SyncVar]
        public Npc rewarderNpc;

        private readonly SyncDictionary<string, float> involvements = new();
        private readonly SyncDictionary<string, int> ranking = new();
        private readonly SyncHashSet<string> taken = new();

        private Involvement[] _involvements;

        public override void OnStartServer()
        {
            base.OnStartServer();
            rewardTier.onSetup.AddListener(Setup);
            rewardTier.onTearDown.AddListener(TearDown);
            spawner.onSpawn.AddListener(OnSpawnRewarder);
            _involvements = transform.GetComponents<Involvement>();
        }

        private void OnSpawnRewarder(GameObject rewarder)
        {
            var npcReward = rewarder.GetComponent<NpcRewards>();
            
            if (npcReward == null) return;

            npcReward.rewardManager = this;
            rewarderNpc = rewarder.GetComponent<Npc>();
        }

        [Server]
        public void Setup()
        {
            ComputeInvolments();
            ComputeRanking();
        }

        [Server]
        public void TearDown()
        {
            involvements.Clear();
            ranking.Clear();
            taken.Clear();
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

        public bool CanTake(Player player)
        {
            return !taken.Contains(player.name);
        }

        [Server]
        public void Take(Player player)
        {
            if (CanTake(player))
            {
                player.gold += gold;
                player.experience.current += experience;
                taken.Add(player.name);
            }
        }
    }
}