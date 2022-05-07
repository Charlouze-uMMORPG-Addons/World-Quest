using UnityEngine;
using UnityEngine.UI;
using WorldQuest;
using WorldQuest.Goals;

public class UINpcRewards : MonoBehaviour
{
    public static UINpcRewards singleton;
    public GameObject panel;
    public Text text;
    public Button rewardButton;

    private RewardManager _rewardManager;

    public UINpcRewards()
    {
        // assign singleton only once (to work with DontDestroyOnLoad when
        // using Zones / switching scenes)
        if (singleton == null) singleton = this;
    }

    private void Update()
    {
        var player = Player.localPlayer;

        if (_rewardManager != null
            && player != null
            && player.target != null
            && player.target == _rewardManager.rewarderNpc
            && Utils.ClosestDistance(player, player.target) <= player.interactionRange)
        {
            text.text = _rewardManager.Text(player);
            rewardButton.onClick.SetListener(() =>
            {
                player.GetComponent<PlayerWorldQuests>().CmdTakeRewards(_rewardManager);
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
        _rewardManager = null;
    }

    public void Activate(RewardManager rewards)
    {
        _rewardManager = rewards;
        panel.SetActive(true);
    }
}