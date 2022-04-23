using System;
using Mirror;
using UnityEngine;

namespace WorldQuest
{
    [RequireComponent(typeof(TierManager))]
    public class DescriptionManager: NetworkBehaviour
    {
        
        [SerializeField]
        public string _description;
        
        public string Description => _description + "\n\n" + _tierManager.CurrentTier.Description;

        private TierManager _tierManager;
        
        private void Start()
        {
            _tierManager = GetComponent<TierManager>();
        }
    }
}