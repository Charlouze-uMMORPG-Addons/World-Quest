using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace WorldQuest
{
    [RequireComponent(typeof(Player))]
    [RequireComponent(typeof(PlayerInventory))]
    public class PlayerWorldQuests : NetworkBehaviour
    {
        public List<Manager> managers = new List<Manager>();

        public bool participating => managers.Count > 0;

        private Player _player;

        private void Start()
        {
            _player = GetComponent<Player>();
        }

        public void Register(Manager manager)
        {
            managers.Add(manager);
        }

        public void Unregister(Manager manager)
        {
            managers.Remove(manager);
        }

        [Command]
        public void CmdTakeRewards(Rewards rewards)
        {
            if (rewards != null &&
                _player.state == "IDLE" &&
                _player.target != null &&
                _player.target.health.current > 0 &&
                _player.target is Npc npc &&
                Utils.ClosestDistance(_player, npc) <= _player.interactionRange)
            {
                _player.gold += rewards.gold;
                _player.experience.current += rewards.experience;
            }
        }
    }
}