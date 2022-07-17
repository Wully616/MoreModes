using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Wully.MoreModes
{
    public class MenuModuleModifiers : MenuModule
    {
        public static MenuModuleModifiers local;
        private Dictionary<ModifierData, Button> buttons;
        private const string buttonPrefab = "Wully.MoreModes.Menu.Button";
        private GameObject buttonGameObject;
        private Transform page1Content;
        private Transform page2Content;
        
        public override void Init(MenuData menuData, Menu menu)
        {
            if (local == null) local = this;
            base.Init(menuData, menu);
            buttons = new Dictionary<ModifierData, Button>();
            //This is based on the MenuOptions prefab, so we sort of destroy it/remake it for ourselves in code
            page1Content = menu.customReferences[0].transform;
            page2Content = menu.customReferences[1].transform;
            
            Catalog.LoadAssetAsync<GameObject>(buttonPrefab,  button => {
                buttonGameObject = button;
                CreateButtons();
            }, "buttonPrefab");

        }

        private void CreateButtons()
        {
            var modifiers = Catalog.GetDataList<ModifierData>();
            var page = page1Content;
            var nextPage = page2Content;
            var currentPage = page;
            foreach (var modifier in modifiers)
            {
                if (modifier.IsSetup)
                {
                    CreateButton(modifier, currentPage);
                    page = nextPage;
                    nextPage = currentPage;
                    currentPage = page;
                }
            }
        }

        private void CreateButton(ModifierData modifierData, Transform parentPage)
        {
            var button = GameObject.Instantiate(buttonGameObject, parentPage);
            button.SetActive(true);
            button.GetComponentInChildren<Text>().text = $"{modifierData.description}";
            var toggleButton = button.GetComponentInChildren<Button>();
            Debug.Log($"Creating button for {modifierData.id}");       
            UpdateColours(toggleButton, modifierData);
            toggleButton.onClick.AddListener(() => {
                ToggleModifer(toggleButton, modifierData);
            });
            buttons.Add(modifierData, toggleButton);
        }

        private void ToggleModifer(Button toggleButton, ModifierData modifierData)
        {
            Debug.Log($"Toggling modifierData button for {modifierData.id}");
            modifierData.Toggle();
            UpdateColours(toggleButton, modifierData);
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

        public void RefreshColour(ModifierData modifierData)
        {
            if (buttons.TryGetValue(modifierData, out var button))
            {
                UpdateColours(button, modifierData);
            }
        }
        
        public void RefreshColours()
        {
            foreach (var button in buttons)
            {
                UpdateColours(button.Value, button.Key);
            }
        }
    }
}
