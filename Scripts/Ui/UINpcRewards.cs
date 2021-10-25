using UnityEngine;
using UnityEngine.UI;
using WorldQuest;

public partial class UINpcRewards : MonoBehaviour
{
    public static UINpcRewards singleton;
    public GameObject panel;
    public Text text;
    public Button rewardButton;

    [HideInInspector]
    public Rewards rewards;

    public UINpcRewards()
    {
        // assign singleton only once (to work with DontDestroyOnLoad when
        // using Zones / switching scenes)
        if (singleton == null) singleton = this;
    }

    void Update()
    {
        var player = Player.localPlayer;

        if (rewards != null
            && player != null
            && player.target != null
            && player.target == rewards.rewarder
            && Utils.ClosestDistance(player, player.target) <= player.interactionRange)
        {
            text.text = rewards.Text(player);
            rewardButton.onClick.SetListener(() =>
            {
                player.GetComponent<PlayerWorldQuests>().CmdTakeRewards(rewards);
                Deactivate();
            });
        }
        else
        {
            Deactivate();
        }
    }

    private void Deactivate()
    {
        panel.SetActive(false);
        rewards = null;
    }

    public void Activate(Rewards rewards)
    {
        this.rewards = rewards;
        panel.SetActive(true);
    }
}