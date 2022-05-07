using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace WorldQuest
{
    [Serializable] public class UnityEventTier : UnityEvent<Tier>
    {
    }

    [RequireComponent(typeof(Players))]
    public class TierManager : NetworkBehaviour
    {
        public Tier[] tiers;

        public UnityEvent onStart;

        public UnityEvent onEnd;

        [SyncVar]
        private int _currentTierIndex;

        public Tier CurrentTier
        {
            get => tiers[_currentTierIndex];
        }

        private void Reset()
        {
            // populating with tiers in children
            tiers = transform.parent.GetComponentsInChildren<Tier>();
        }

        public override void OnStartServer()
        {
            if (tiers.Length == 0)
            {
                gameObject.SetActive(false);
                Debug.LogErrorFormat("World quest '{0}' has no tier... there must be a problem !", name);
            }
            else
            {
                foreach (var tier in tiers)
                {
                    TearDown(tier);
                }

                Restart();
            }
        }

        [ServerCallback]
        private void Update()
        {
            if (CurrentTier.IsFulfilled())
            {
                if (_currentTierIndex == tiers.Length - 1)
                {
                    onEnd.Invoke();
                }

                Debug.LogFormat("Tier '{0}' is fulfilled.", CurrentTier.name);
                TearDown(CurrentTier);
                NextTier();
            }

            if (!CurrentTier.active)
            {
                if (_currentTierIndex == 0)
                {
                    onStart.Invoke();
                }

                Setup(CurrentTier);
            }
        }

        private void Setup(Tier tier)
        {
            tier.Setup();
        }

        private void TearDown(Tier tier)
        {
            tier.TearDown();
        }

        private void NextTier()
        {
            _currentTierIndex++;
            if (NoMoreTier())
            {
                Restart();
            }
        }

        private void Restart()
        {
            _currentTierIndex = 0;
        }

        private bool NoMoreTier()
        {
            return tiers.Length <= _currentTierIndex;
        }
    }
}