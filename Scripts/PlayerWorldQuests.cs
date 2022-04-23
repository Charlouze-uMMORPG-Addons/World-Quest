using Mirror;
using UnityEngine;

namespace WorldQuest
{
    [RequireComponent(typeof(Player))]
    [RequireComponent(typeof(PlayerInventory))]
    public class PlayerWorldQuests : NetworkBehaviour
    {
        public SyncList<DescriptionManager> managers = new SyncList<DescriptionManager>();

        public bool participating => managers.Count > 0;

        private Player _player;

        private void Start()
        {
            _player = GetComponent<Player>();
        }

        [Server]
        public void Add(GameObject worldQuestGo)
        {
            managers.Add(worldQuestGo.GetComponent<DescriptionManager>());
        }

        [Server]
        public void Remove(GameObject worldQuestGo)
        {
            managers.Remove(worldQuestGo.GetComponent<DescriptionManager>());
        }

        [Command]
        public void CmdTakeRewards(Rewards rewards)
        {
            if (rewards != null &&
                rewards.CanTake(_player) &&
                _player.state == "IDLE" &&
                _player.target != null &&
                _player.target.health.current > 0 &&
                _player.target is Npc npc &&
                Utils.ClosestDistance(_player, npc) <= _player.interactionRange)
            {
                rewards.Take(_player);
            }
        }
    }
}