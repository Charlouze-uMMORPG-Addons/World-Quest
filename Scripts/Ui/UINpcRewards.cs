using UnityEngine;
using UnityEngine.UI;
using WorldQuest;
using WorldQuest.Goals;

public partial class UINpcRewards : MonoBehaviour
{
    public static UINpcRewards singleton;
    public GameObject panel;
    public Text text;
    public Button rewardButton;

    [HideInInspector]
    private RewardGoal _rewardGoal;

    public UINpcRewards()
    {
        // assign singleton only once (to work with DontDestroyOnLoad when
        // using Zones / switching scenes)
        if (singleton == null) singleton = this;
    }

    void Update()
    {
        var player = Player.localPlayer;

        if (_rewardGoal != null
            && player != null
            && player.target != null
            && player.target == _rewardGoal.rewarderNpc
            && Utils.ClosestDistance(player, player.target) <= player.interactionRange)
        {
            text.text = _rewardGoal.Text(player);
            rewardButton.onClick.SetListener(() =>
            {
                player.GetComponent<PlayerWorldQuests>().CmdTakeRewards(_rewardGoal);
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
        _rewardGoal = null;
    }

    public void Activate(RewardGoal rewards)
    {
        this._rewardGoal = rewards;
        panel.SetActive(true);
    }
}