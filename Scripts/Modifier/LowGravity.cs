using System.Collections;
using UnityEngine;
using Wully.MoreModes;

namespace Wully.MoreModes
{
    public class LowGravity : ModifierData
    {
        public float gravity = -4.9f;
        private Vector3 gravityForce;

        public static LowGravity Instance;
        
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
            if (HighGravity.Instance.IsEnabled)
            {
                //Disable HighGravity first
                HighGravity.Instance.Toggle();
                MenuModuleModifiers.local.RefreshColour(HighGravity.Instance);
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
