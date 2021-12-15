using Mirror;

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

        [Server]
        public virtual void RegisterPlayer(Player player)
        {
            // NoOp
        }

        [Server]
        public virtual void UnregisterPlayer(Player player)
        {
            // NoOp
        }

        public virtual string Description => "";
    }
}