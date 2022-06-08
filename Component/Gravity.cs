using System.Collections;
using GameModeLoader.Data;
using UnityEngine;

namespace GameModeLoader.Component
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
