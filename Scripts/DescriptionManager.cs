using Mirror;
using UnityEngine;

namespace WorldQuest
{
    [RequireComponent(typeof(TierManager))]
    public class DescriptionManager : NetworkBehaviour
    {
        [SerializeField]
        private string _nameOverride;

        [SerializeField]
        private string _description;

        private TierManager _tierManager;

        public string Description => _description + "\n\n" + _tierManager.CurrentTier.Description;
        public object Name => string.IsNullOrWhiteSpace(_nameOverride) ? transform.parent.name : _nameOverride;

        private void Awake()
        {
            _tierManager = GetComponent<TierManager>();
        }
    }
}