using UnityEngine;

namespace WorldQuest.Goals
{
    public class DelayGoal : Goal
    {
        public float delay = 60;

        private float _setupTime;

        public override void Setup()
        {
            base.Setup();
            _setupTime = Time.time;
        }

        public override bool IsFulfilled()
        {
            return Time.time - _setupTime > delay;
        }

        public override string Description => $"{Mathf.FloorToInt(delay + _setupTime - Time.time)} seconds left";
    }
}