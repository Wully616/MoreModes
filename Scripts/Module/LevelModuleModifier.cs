using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine.EventSystems;
using Wully.MoreModes.Data;

namespace Wully.MoreModes
{
    public class LevelModuleModifier : LevelModule
    {
        public static LevelModuleModifier local;

        protected List<Modifier> modifiers;
        
        public override IEnumerator OnLoadCoroutine()
        {
            if (local != null) yield break;
            local = this;
            modifiers = new List<Modifier>();
            yield return base.OnLoadCoroutine();
        }

        public override void Update()
        {
            base.Update();
            UpdateModifiers();
        }

        public bool AddModifier(Modifier modifier)
        {
            if (modifier.IsSetup) //dont add the modifier if it hasnt been setup
            {
                var modifiersCount = modifiers.Count;
                for (int i = 0; i < modifiersCount; i++)
                {
                    if (modifiers[i] == modifier) return false;
                }
                modifiers.Add(modifier);
                return true;
            }
            return false;
        }
        
        public bool RemoveModifier(Modifier modifier)
        {
            return modifiers.Remove(modifier);
        }
        
        private void UpdateModifiers()
        {
            if (modifiers == null) modifiers = new List<Modifier>();
            var modifiersCount = modifiers.Count;
            for (int i = 0; i < modifiersCount; i++)
            {
                var modifier = modifiers[i];
                modifier.Update();
                
            }
        }
    }
}
