using System.Collections;
using UnityEngine;
using Wully.MoreModes;

namespace Wully.MoreModes
{
    public class HighGravity : ModifierData
    {
        public float gravity = -14.7f;
        private Vector3 gravityForce;

        public static HighGravity Instance;
        
        public override void Init()
        {
            if (Instance == null)
            {
                base.Init();
                Instance = this;
                // bit hacky, but we only one 1 modifier, if local isnt set, this modifier isnt Setup
                local = this;
            }
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            if (LowGravity.Instance.IsEnabled)
            {
                //Disable lowgravity first
                LowGravity.Instance.Toggle();
                MenuModuleModifiers.local.RefreshColour(LowGravity.Instance);
            }
            
            gravityForce = Physics.gravity;
            Physics.gravity = new Vector3(0f, gravity, 0f);
        }

        protected override void OnDisable() {
            base.OnDisable();
            Physics.gravity = gravityForce;
        }
        
    }
}
