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

        public UnityEventTier onTierSetup;

        public UnityEventTier onTierTearDown;

        private Players _players;

        [SyncVar]
        private int _currentTierIndex;

        public Tier CurrentTier
        {
            get => tiers[_currentTierIndex];
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
                _players = GetComponent<Players>();
                _players.onPlayerEnter.AddListener(player => CurrentTier.Register(player));
                _players.onPlayerLeave.AddListener(player => CurrentTier.Unregister(player));

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
            _players.ForEach(player => tier.Register(player));
            onTierSetup.Invoke(tier);
        }

        private void TearDown(Tier tier)
        {
            onTierTearDown.Invoke(tier);
            _players.ForEach(player => tier.Unregister(player));
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