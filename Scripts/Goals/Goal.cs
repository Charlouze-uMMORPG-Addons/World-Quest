using Mirror;
using UnityEngine;

namespace WorldQuest
{
    public abstract class Goal : NetworkBehaviour
    {
        public abstract bool IsFulfilled();

        [Server]
        public virtual void Setup()
        {
            // NoOp
        }

        [Server]
        public virtual void TearDown()
        {
            // NoOp
        }

        public virtual string Description => "";
    }
}