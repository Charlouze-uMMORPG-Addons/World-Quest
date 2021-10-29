using Mirror;

namespace WorldQuest
{
    public abstract class Goal : NetworkBehaviour
    {
        public abstract bool IsFulfilled();

        public virtual void Setup()
        {
            // NoOp
        }

        public virtual void TearDown()
        {
            // NoOp
        }

        public virtual void RegisterPlayer(Player player)
        {
            // NoOp
        }

        public virtual void UnregisterPlayer(Player player)
        {
            // NoOp
        }

        public virtual string Description => "";
    }
}