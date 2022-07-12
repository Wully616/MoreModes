using ThunderRoad;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Wully.MoreModes
{
    public class MenuModuleModifiers : MenuModule
    {
        
        private const string buttonPrefab = "Wully.MoreModes.Menu.Button";
        private GameObject buttonGameObject;
        private Transform page1Content;
        private Transform page2Content;
        public override void Init(MenuData menuData, Menu menu)
        {
            base.Init(menuData, menu);
            //This is based on the MenuOptions prefab, so we sort of destroy it/remake it for ourselves in code
            page1Content = menu.customReferences[0].transform;
            page2Content = menu.customReferences[1].transform;
            
            Catalog.InstantiateAsync(buttonPrefab, Vector3.zero, Quaternion.identity, menu.transform, button => {
                buttonGameObject = button;
                buttonGameObject.SetActive(false);
                CreateButtons();
            }, "buttonPrefab");

        }

        private void CreateButtons()
        {
            var modifiers = Catalog.GetDataList<Modifier>();
            foreach (var modifier in modifiers)
            {
               CreateButton(modifier);
            }
        }

        private void CreateButton(Modifier modifier)
        {
            var button = GameObject.Instantiate(buttonGameObject, page1Content);
            button.SetActive(true);
            button.GetComponentInChildren<Text>().text = $"{modifier.description}";
            var toggleButton = button.GetComponentInChildren<Button>();
            Debug.Log($"Creating button for {modifier.Local.id}");       
            UpdateColours(toggleButton, modifier);
            toggleButton.onClick.AddListener(() => {
                ToggleModifer(toggleButton, modifier);
            });
        }

        private void ToggleModifer(Button toggleButton, Modifier modifier)
        {
            Debug.Log($"Toggling modifier button for {modifier.Local.id}");
            modifier.Local.Toggle();
            UpdateColours(toggleButton, modifier);
        }
        
        private void UpdateColours(Button toggleButton, Modifier modifier)
        {
            var cb = toggleButton.colors;
            if (modifier.Local.IsEnabled)
            {
                cb.normalColor = Color.green;
            }
            else
            {
                cb.normalColor = Color.red;
            }
            toggleButton.colors = cb;
        }
    }
}
