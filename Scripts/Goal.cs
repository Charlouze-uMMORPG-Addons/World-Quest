using Mirror;

namespace WorldQuest
{
    public abstract class Goal : NetworkBehaviour
    {
        [SyncVar]
        public int progress;

        public abstract bool IsFulfilled();

        public abstract void RegisterPlayer(Player player);

        public abstract void UnregisterPlayer(Player player);

        public abstract string Description { get; }

        public virtual void Setup()
        {
            progress = 0;
        }
    }
}