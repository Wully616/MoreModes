using ThunderRoad;
using UnityEngine;

namespace Wully.MoreModes
{
    public abstract class Modifier : CustomData
    {
        public string description;
        
        public Modifier local;
        public Modifier Local => local;
        private bool isEnabled;

        public bool IsEnabled => local.isEnabled;

        public override void Init()
        {
            base.Init();
            //there can only be one valid instance of a modifier
            if (Local != null)
            {
                Debug.LogError($"There is a duplicate Modifier with the id of {id} and type {this.GetType()}. There can only be one instance of each Modifier");
            }
            else
            {
                local = this;
            }
        }

        public void Toggle()
        {
            Debug.Log($"Toggling {this.Local.id}");
            if (IsEnabled)
            {
                local.Disable();
                local.isEnabled = false;
            }
            else
            {
                local.Enable();
                local.isEnabled = true;
            }
        }

        
        public void Enable()
        {
            OnEnable();
            LevelModuleModifier.local.AddModifier(this);
            EventManager.onLevelLoad += OnLevelLoad;
            EventManager.onLevelUnload += OnLevelUnload;
            EventManager.onPossess += OnPossess;
            EventManager.onUnpossess += OnUnPossess;
        }
        
        public void Disable()
        {
            OnDisable();
            LevelModuleModifier.local.RemoveModifier(this);
            EventManager.onLevelLoad -= OnLevelLoad;
            EventManager.onLevelUnload -= OnLevelUnload;
            EventManager.onPossess -= OnPossess;
            EventManager.onUnpossess -= OnUnPossess;
        }
                
        protected virtual void OnEnable()
        {
            
        }
        
        protected virtual void OnDisable()
        {
            
        }

        public virtual void Update()
        {
            
        }
        
        protected virtual void OnLevelUnload(LevelData levelData, EventTime eventTime)
        {
        }

        protected virtual void OnLevelLoad(LevelData levelData, EventTime eventTime)
        {
        }
        
        protected virtual void OnUnPossess(Creature creature, EventTime eventTime)
        {
        }
        protected virtual void OnPossess(Creature creature, EventTime eventTime)
        {
        }
    }
}
