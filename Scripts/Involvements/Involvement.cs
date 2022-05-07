using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace WorldQuest
{
    [RequireComponent(typeof(TierManager))]
    public abstract class Involvement : NetworkBehaviour
    {
        public float rate = 1f;

        public readonly Dictionary<string, float> scores = new Dictionary<string, float>();

        protected TierManager _tierManager;
        
        public override void OnStartServer()
        {
            _tierManager = GetComponent<TierManager>();
            _tierManager.onStart.AddListener(OnStart);
        }

        [Server]
        public void OnStart()
        {
            if (Debug.isDebugBuild)
            {
                foreach (var nameScorePair in scores)
                {
                    Debug.LogFormat("\n{0} - player: {1}\tscore: {2}", GetType(), nameScorePair.Key,
                        nameScorePair.Value);
                }
            }

            scores.Clear();
        }

        [Server]
        protected void Add(Entity entity, float score)
        {
            if (!scores.ContainsKey(entity.name))
            {
                scores[entity.name] = 0;
            }

            var totalScore = scores[entity.name] + score * rate;
            Debug.LogFormat("{0} scored {1} for a total of {2}", entity.name, score, totalScore);
            scores[entity.name] = totalScore;
        }
    }
}