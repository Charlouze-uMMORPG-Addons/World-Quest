using Mirror;
using TMPro;
using UnityEngine;
using WorldQuest.Goals;

namespace WorldQuest
{
    public class NpcRewards : NpcOffer
    {
        [Header("Text Meshes")]
        public TextMeshPro questOverlay;

        [HideInInspector]
        [SyncVar]
        public RewardGoal rewardGoal;

        public override bool HasOffer(Player player) => rewardGoal.CanTake(player);

        public override string GetOfferName() => "Rewards";

        public override void OnSelect(Player player)
        {
            UINpcRewards.singleton.Activate(rewardGoal);
            UINpcDialogue.singleton.panel.SetActive(false);
        }

        private void Update()
        {
            // setup overlay only if needed
            if (isServerOnly) return;

            var player = Player.localPlayer;

            if (player != null && rewardGoal.CanTake(player))
            {
                questOverlay.text = "!";
            }
            else
            {
                questOverlay.text = "";
            }
        }
    }
}