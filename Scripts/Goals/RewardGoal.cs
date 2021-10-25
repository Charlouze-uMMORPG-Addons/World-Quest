namespace WorldQuest.Goals
{
    public class RewardGoal : DelayGoal
    {
        public Rewards rewards;

        public override void Setup()
        {
            base.Setup();
            rewards.Prepare();
        }
    }
}