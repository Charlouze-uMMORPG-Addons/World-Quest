using Mirror;

namespace WorldQuest.Goals
{
    public abstract class ProgressGoal : Goal
    {
        [field: SyncVar]
        protected int _progress { get; private set; }

        protected abstract bool IsIncrementing { get; }

        protected abstract int StartProgress { get; }

        protected abstract int EndProgress { get; }

        [Server]
        public override void Setup()
        {
            UpdateProgress(StartProgress);
        }

        public override bool IsFulfilled()
        {
            return IsIncrementing ? _progress >= EndProgress : _progress <= EndProgress;
        }

        [Server]
        protected void UpdateProgress(int newProgress)
        {
            if (_progress != newProgress) _progress = newProgress;
        }
    }
}