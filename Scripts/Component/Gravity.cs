using System.Collections;
using UnityEngine;
using Wully.MoreModes.Data;

namespace Wully.MoreModes.Component
{
    public class Gravity : LevelModuleOptional
    {
        public float gravity = -9.81f;
        private Vector3 gravityForce;

        public override IEnumerator OnLoadCoroutine()
        {
            SetId();
            if (IsEnabled())
            {
                gravityForce = Physics.gravity;
                Physics.gravity = new Vector3(0f, gravity, 0f);
            }
            
            yield break;
        }

        public override void OnUnload()
        {
            base.OnUnload();
            if (IsEnabled())
            {
                Physics.gravity = gravityForce;
            }
        }
    }
}
