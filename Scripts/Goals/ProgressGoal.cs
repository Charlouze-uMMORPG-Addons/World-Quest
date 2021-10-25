using Mirror;
using UnityEngine;

namespace WorldQuest.Goals
{
    public abstract class ProgressGoal : Goal
    {
        [SyncVar]
        public int progress;

        public override void Setup()
        {
            progress = 0;
        }

        public override bool IsFulfilled()
        {
           return progress >= EndProgress;
        }

        protected abstract int EndProgress { get; }
    }
}