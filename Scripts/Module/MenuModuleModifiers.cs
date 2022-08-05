using System;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Wully.MoreModes
{
    public class MenuModuleModifiers : MenuModule
    {
        public static MenuModuleModifiers local;
        private Dictionary<ModifierData, Toggle> buttons;
        private const string buttonPrefab = "Wully.MoreModes.Menu.Button";
        private const string categoryPrefab = "Wully.MoreModes.Menu.Category";
        private GameObject buttonGameObject;
        private GameObject categoryGameObject;
        private Transform page1Content;
        private Transform page2Content;
        
        public override void Init(MenuData menuData, Menu menu)
        {
            if (local == null) local = this;
            base.Init(menuData, menu);
            buttons = new Dictionary<ModifierData, Toggle>();
            //This is based on the MenuOptions prefab, so we sort of destroy it/remake it for ourselves in code
            page1Content = (Transform)menu.customReferences[0].transform;
            page2Content = (Transform)menu.customReferences[1].transform;
            
            Catalog.LoadAssetAsync<GameObject>(buttonPrefab,  button => {
                buttonGameObject = button;
                Catalog.LoadAssetAsync<GameObject>(categoryPrefab,  category => {
                    categoryGameObject = category;
                    CreateButtons();
                }, "buttonPrefab");
                
            }, "buttonPrefab");

        }

        private void CreateButtons()
        {
            var modifiers = Catalog.GetDataList<ModifierData>()
                .OrderBy(m => m.category)
                .ThenBy(m => m.description)
                .ToList();
            var currentPage = page1Content;
            int modifiersCount = modifiers.Count;
            var half = modifiersCount / 2;
            ModifierData.Category currentCategory = modifiers[0].category;

            CreateCategory(currentCategory, currentPage);
            for (var i = 0; i < modifiersCount; i++)
            {
                var modifier = modifiers[i];
                
                if (modifier.IsSetup)
                {
                    if (modifier.category != currentCategory)
                    {
                        currentCategory = modifier.category;
                        CreateCategory(currentCategory, currentPage);
                    }
                    CreateButton(modifier, currentPage);
                    if (i > half)
                    {
                        currentPage = page2Content;
                    }
                }
            }
        }
        private void CreateCategory(ModifierData.Category category, Transform parentPage)
        {
            var go = GameObject.Instantiate(categoryGameObject, parentPage);
            go.SetActive(true);
            go.GetComponentInChildren<Text>().text = $"{category.ToString()}";
        }
        
        private void CreateButton(ModifierData modifierData, Transform parentPage)
        {
            var button = GameObject.Instantiate(buttonGameObject, parentPage);
            button.SetActive(true);
            button.GetComponentInChildren<Text>().text = $"{modifierData.description}";
            var toggleButton = button.GetComponentInChildren<Toggle>();
            toggleButton.onValueChanged.AddListener((enabled => {
                ToggleModifer(enabled, modifierData);
            }));
            
            buttons.Add(modifierData, toggleButton);
        }

        private void ToggleModifer(bool value, ModifierData modifierData)
        {
            if (value)
            {
                modifierData.Enable();
            }
            else
            {
                modifierData.Disable();
            }
        }
        
        private void UpdateColours(Button toggleButton, ModifierData modifierData)
        {
            var cb = toggleButton.colors;
            if (modifierData.IsEnabled)
            {
                cb.normalColor = Color.green;
            }
            else
            {
                cb.normalColor = Color.red;
            }
            toggleButton.colors = cb;
        }

        public void RefreshToggle(ModifierData modifierData)
        {
            if (buttons.TryGetValue(modifierData, out var button))
            {
                button.isOn = modifierData.IsEnabled;
            }
        }
        
        // public void RefreshColours()
        // {
        //     foreach (var button in buttons)
        //     {
        //         UpdateColours(button.Value, button.Key);
        //     }
        // }
    }
}
