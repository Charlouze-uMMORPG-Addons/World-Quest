using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace WorldQuest
{
    [Serializable] public class UnityEventTier : UnityEvent<Tier> {}
    
    [RequireComponent(typeof(NetworkIdentity))]
    public class Manager : NetworkBehaviour
    {
        public string description;
        
        public Tier[] tiers;

        public UnityEventTier onTierSetup;

        public UnityEventTier onTierTearDown;

        private int _currentTierIndex;

        public Tier CurrentTier
        {
            get => tiers[_currentTierIndex];
        }

        [ServerCallback]
        private void Start()
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
            }
        }

        [ServerCallback]
        private void Update()
        {
            if (CurrentTier.IsFulfilled())
            {
                Debug.LogFormat("Tier '{0}' is fulfilled.", CurrentTier.name);
                TearDown(CurrentTier);
                NextTier();
            }

            if (!CurrentTier.gameObject.activeSelf)
            {
                Debug.LogFormat("Prepare tier '{0}'", CurrentTier.name);
                Setup(CurrentTier);
            }
        }

        private void Setup(Tier tier)
        {
            tier.gameObject.SetActive(true);
            onTierSetup.Invoke(tier);
        }

        private void TearDown(Tier tier)
        {
            tier.gameObject.SetActive(false);
            onTierTearDown.Invoke(tier);
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