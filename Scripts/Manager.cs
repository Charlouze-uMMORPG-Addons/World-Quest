using System;
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

        public UnityEventTier onTierSetup;

        public UnityEventTier onTierTearDown;

        private int _currentTierIndex;

        public Tier CurrentTier
        {
            get => tiers[_currentTierIndex];
        }

        public string Description => _description + "\n\n" + CurrentTier.Description;

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

        [ClientCallback]
        public void Register(Player player)
        {
            if (player == Player.localPlayer)
            {
                player.GetComponent<PlayerWorldQuests>().managers.Add(this);
            }
        }

        [ClientCallback]
        public void Unregister(Player player)
        {
            if (player == Player.localPlayer)
            {
                player.GetComponent<PlayerWorldQuests>().managers.Remove(this);
            }
        }

        private void Setup(Tier tier)
        {
            tier.gameObject.SetActive(true);
            tier.Setup();
            onTierSetup.Invoke(tier);
        }

        private void TearDown(Tier tier)
        {
            onTierTearDown.Invoke(tier);
            tier.TearDown();
            tier.gameObject.SetActive(false);
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