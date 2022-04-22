﻿using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace WorldQuest
{
    [Serializable] public class UnityEventTier : UnityEvent<Tier>
    {
    }

    [RequireComponent(typeof(NetworkIdentity))]
    public class Manager : NetworkBehaviour
    {
        [SerializeField]
        public string _description;

        public Tier[] tiers;

        public UnityEvent onStart;

        public UnityEvent onEnd;

        public UnityEventTier onTierSetup;

        public UnityEventTier onTierTearDown;

        [SyncVar]
        private int _currentTierIndex;

        public Tier CurrentTier
        {
            get => tiers[_currentTierIndex];
        }

        public string Description => _description + "\n\n" + CurrentTier.Description;

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

                Debug.LogFormat("Prepare tier '{0}'", CurrentTier.name);
                Setup(CurrentTier);
            }
        }

        [Server]
        public void Register(Player player)
        {
            player.GetComponent<PlayerWorldQuests>().Register(this);
        }

        [Server]
        public void Unregister(Player player)
        {
            player.GetComponent<PlayerWorldQuests>().Unregister(this);
        }

        private void Setup(Tier tier)
        {
            tier.Setup();
            onTierSetup.Invoke(tier);
        }

        private void TearDown(Tier tier)
        {
            onTierTearDown.Invoke(tier);
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