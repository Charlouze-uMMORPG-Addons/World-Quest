using System;
using Mirror;
using UnityEngine;
using WorldQuest.Goals;

namespace WorldQuest
{
    [RequireComponent(typeof(Player))]
    public class PlayerWorldQuests : NetworkBehaviour
    {
        public int Count => worldQuests.Count;

        public bool participating => Count > 0;

        private readonly SyncList<GameObject> worldQuests = new();

        private Player _player;

        private void Awake()
        {
            _player = GetComponent<Player>();
        }

        public DescriptionManager GetDescriptionManager(int wqIndex)
        {
            if (worldQuests.Count > wqIndex)
            {
                return worldQuests[wqIndex].GetComponent<DescriptionManager>();
            }

            throw new ArgumentException("There are no world quest for index " + wqIndex);
        }

        [Server]
        public void Add(GameObject worldQuestGo)
        {
            worldQuests.Add(worldQuestGo);
        }

        [Server]
        public void Remove(GameObject worldQuestGo)
        {
            worldQuests.Remove(worldQuestGo);
        }

        [Command]
        public void CmdTakeRewards(RewardManager rewardManager)
        {
            if (rewardManager != null &&
                rewardManager.CanTake(_player) &&
                _player.state == "IDLE" &&
                _player.target != null &&
                _player.target.health.current > 0 &&
                _player.target is Npc npc &&
                Utils.ClosestDistance(_player, npc) <= _player.interactionRange)
            {
                rewardManager.Take(_player);
            }
        }
    }
}