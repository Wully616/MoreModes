using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine.EventSystems;
using Wully.MoreModes;

namespace Wully.MoreModes
{
    public class LevelModuleModifier : LevelModule
    {
        public static LevelModuleModifier local;

        protected List<ModifierData> modifiers;
        
        public override IEnumerator OnLoadCoroutine()
        {
            if (local != null) yield break;
            local = this;
            modifiers = new List<ModifierData>();
            yield return base.OnLoadCoroutine();
        }

        public override void Update()
        {
            base.Update();
            UpdateModifiers();
        }

        public bool AddModifier(ModifierData modifierData)
        {
            if (modifierData.IsSetup) //dont add the modifierData if it hasnt been setup
            {
                var modifiersCount = modifiers.Count;
                for (int i = 0; i < modifiersCount; i++)
                {
                    if (modifiers[i] == modifierData) return false;
                }
                modifiers.Add(modifierData);
                return true;
            }
            return false;
        }
        
        public bool RemoveModifier(ModifierData modifierData)
        {
            return modifiers.Remove(modifierData);
        }
        
        private void UpdateModifiers()
        {
            if (modifiers == null) modifiers = new List<ModifierData>();
            var modifiersCount = modifiers.Count;
            for (int i = 0; i < modifiersCount; i++)
            {
                var modifier = modifiers[i];
                modifier.Update();
                
            }
        }
    }
}
