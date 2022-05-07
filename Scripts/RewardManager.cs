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

        [SerializeField] [TextArea(1, 30)] public string text;
        public long gold;
        public long experience;

        public Involvement[] _involvements;

        [HideInInspector]
        [SyncVar]
        public Npc rewarderNpc;

        private readonly SyncDictionary<string, float> playerInvolvements = new();
        private readonly SyncDictionary<string, int> playerRanking = new();
        private readonly SyncHashSet<string> playerRewardTaken = new();

        private void Reset()
        {
            _involvements = GetComponents<Involvement>();
        }

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

            if (npcReward == null)
            {
                return;
            }

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
            playerInvolvements.Clear();
            playerRanking.Clear();
            playerRewardTaken.Clear();
        }

        private void ComputeInvolments()
        {
            foreach (var involvement in _involvements)
            {
                foreach (var nameScorePair in involvement.scores)
                {
                    if (!playerInvolvements.ContainsKey(nameScorePair.Key))
                    {
                        playerInvolvements[nameScorePair.Key] = 0;
                    }

                    playerInvolvements[nameScorePair.Key] += nameScorePair.Value;
                }
            }
        }

        private void ComputeRanking()
        {
            var involvementList = new List<KeyValuePair<string, float>>();
            foreach (var involvement in playerInvolvements)
            {
                involvementList.Add(involvement);
            }

            involvementList.Sort((involvement1, involvement2) => involvement2.Value.CompareTo(involvement1.Value));
            for (var i = 0; i < involvementList.Count; i++)
            {
                playerRanking[involvementList[i].Key] = i + 1;
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
            if (playerRanking.ContainsKey(player.name))
            {
                return playerRanking[player.name];
            }

            return playerRanking.Count + 1;
        }

        public bool CanTake(Player player)
        {
            return !playerRewardTaken.Contains(player.name);
        }

        [Server]
        public void Take(Player player)
        {
            if (CanTake(player))
            {
                player.gold += gold;
                player.experience.current += experience;
                playerRewardTaken.Add(player.name);
            }
        }
    }
}