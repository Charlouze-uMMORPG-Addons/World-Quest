using System;
using Mirror;
using UnityEngine;

namespace WorldQuest.Goals
{
    public class DelayGoal : Goal
    {
        public float delay = 60;

        private float _setupTime;

        [SyncVar]
        private int _remainingTime;
        
        public override void Setup()
        {
            base.Setup();
            _setupTime = Time.time;
        }

        public override bool IsFulfilled()
        {
            return Time.time - _setupTime > delay;
        }

        [ServerCallback]
        private void Update()
        {
            var remainingTime = Mathf.FloorToInt(delay + _setupTime - Time.time);
            if (_remainingTime != remainingTime)
            {
                _remainingTime = remainingTime;
            }
        }

        public override string Description => $"{_remainingTime} seconds left";
    }
}