using Mirror;
using UnityEngine;

namespace WorldQuest.Goals
{
    public class DelayGoal : ProgressGoal
    {
        public int delay = 60;

        private float _setupTime;

        public override string Description => $"{_progress} seconds left";

        protected override bool IsIncrementing => false;

        protected override int StartProgress => delay;

        protected override int EndProgress => 0;

        [ServerCallback]
        private void Update()
        {
            UpdateProgress(Mathf.CeilToInt(delay + _setupTime - Time.time));
        }

        [Server]
        public override void Setup()
        {
            base.Setup();
            _setupTime = Time.time;
        }
    }
}