using ThunderRoad;
using UnityEngine;

namespace Wully.MoreModes
{
    public abstract class ModifierData : CustomData
    {
        /// <summary>
        /// Description of this modifier which appears in the book
        /// </summary>
        public string description;
        
        // A local variable which should only be non-null for the real instance of this modifier
        protected ModifierData local;
        
        private bool isEnabled;
        /// <summary>
        /// Returns true if this modifier is enabled
        /// </summary>
        public bool IsEnabled => local.isEnabled;
        /// <summary>
        /// Returns true if this modifier has been setup
        /// </summary>
        public bool IsSetup => local != null;
        
        public void Toggle()
        {
            if (!IsSetup) return;
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
            if (!IsSetup) return;
            OnEnable();
            LevelModuleModifier.local.AddModifier(this);
            EventManager.onLevelLoad += OnLevelLoad;
            EventManager.onLevelUnload += OnLevelUnload;
            EventManager.onPossess += OnPossess;
            EventManager.onUnpossess += OnUnPossess;
        }
        
        public void Disable()
        {
            if (!IsSetup) return;
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

        // Called each frame
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
