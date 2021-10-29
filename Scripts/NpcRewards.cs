using TMPro;
using UnityEngine;

namespace WorldQuest
{
    public class NpcRewards : NpcOffer
    {
        [Header("Text Meshes")]
        public TextMeshPro questOverlay;

        [HideInInspector]
        public Rewards rewards;

        public override bool HasOffer(Player player) => rewards.CanTake(player);

        public override string GetOfferName() => "Rewards";

        public override void OnSelect(Player player)
        {
            UINpcRewards.singleton.Activate(rewards);
            UINpcDialogue.singleton.panel.SetActive(false);
        }

        private void Update()
        {
            // setup overlay only if needed
            if (isServerOnly) return;

            var player = Player.localPlayer;

            if (player != null && rewards.CanTake(player))
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