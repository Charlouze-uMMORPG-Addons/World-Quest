using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace WorldQuest
{
    public class Tier : NetworkBehaviour
    {
        [SerializeField]
        private string _description;

        public Goal[] goals;

        public UnityEvent onSetup;
        
        public UnityEvent onTearDown;

        [HideInInspector]
        public bool active;

        private void Reset()
        {
            // Populating with goals that are on this GO
            goals = GetComponents<Goal>();
        }

        public string Description
        {
            get
            {
                var desc = _description + "\n";
                foreach (var goal in goals)
                {
                    desc = desc + "\n" + goal.Description;
                }

                return desc;
            }
        }

        private void Awake()
        {
            goals = GetComponents<Goal>();
        }

        [Server]
        public void Setup()
        {
            Debug.LogFormat("Setting up tier '{0}'", name);
            
            foreach (var goal in goals)
            {
                goal.Setup();
            }
            
            onSetup.Invoke();

            active = true;
        }

        [Server]
        public void TearDown()
        {
            active = false;
            
            onTearDown.Invoke();

            foreach (var goal in goals)
            {
                goal.TearDown();
            }

            Debug.LogFormat("Tearing down tier '{0}'", name);
        }

        public bool IsFulfilled()
        {
            var fulfilled = true;
            foreach (var goal in goals)
            {
                fulfilled &= goal.IsFulfilled();
            }

            return fulfilled;
        }
    }
}