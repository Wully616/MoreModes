using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
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
            EventManager.onLevelLoad += OnLevelLoad;
            PlayerControl.local.comboButton = PlayerControl.ComboButton.ExternalViewLock;
            yield return base.OnLoadCoroutine();
            
        }
        private void OnLevelLoad(LevelData leveldata, EventTime eventTime)
        {
            if(eventTime == EventTime.OnStart ) return;
            Level.current.StartCoroutine(AddScroll());
        }
        

        private IEnumerator AddScroll()
        {
            while (MenuBook.local == null && MenuBook.local.navBar == null)
            {
                yield return null;
            }
            if (!MenuBook.local.navBar.TryGetComponent(out ScrollRect scrollRect))
            {
                scrollRect = MenuBook.local.navBar.AddComponent<ScrollRect>();
                scrollRect.vertical = false;
                scrollRect.viewport = MenuBook.local.navBar.GetComponent<RectTransform>();
                scrollRect.content = scrollRect.transform.GetChild(2).GetComponent<RectTransform>();
            }
            
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
