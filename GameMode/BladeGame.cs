using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameModeLoader.Data;
using ThunderRoad;
using UnityEngine;

namespace GameModeLoader.GameMode
{
    /// <summary>
    ///     This survival mode inherits from the base games survival game mode
    /// </summary>
    public class BladeGame : LevelModuleOptional
    {
        public bool allowStealing = false;
        private WaitForSeconds waitFor0_5Second = new WaitForSeconds(0.5f);
        private int idx = 0;
        private string[] itemIds = {"DaggerCommon", "AxeShortCommon", "SwordShortCommon", "AxeShortWar" };
        private EffectData rewardFxData;

        public string rewardFxId = "SurvivalMode.RewardFx";
        private List<ItemData.Type> allowedItemTypes = new List<ItemData.Type> {
            ItemData.Type.Prop,
            ItemData.Type.Body,
            ItemData.Type.Misc,
            ItemData.Type.Wardrobe,
            ItemData.Type.Potion
        };

        public override IEnumerator OnLoadCoroutine()
        {
            SetId();
            if (!IsEnabled()) yield break;
            EventManager.onCreatureKill += EventManager_onCreatureKill;
            EventManager.onPossess += EventManager_onPossess;
            EventManager.onUnpossess += EventManager_onUnpossess;
            rewardFxData = Catalog.GetData<EffectData>(rewardFxId);
            itemIds = Catalog.GetDataList(Catalog.Category.Item)
                .Cast<ItemData>().Where(d => d.type == ItemData.Type.Weapon && d.id != "Torch" && d.slot != "Arrow" && d.slot != "Bow" && d.damagers.Count > 0).OrderBy(d => d.tier)
                .ThenBy(d => d.mass)
                .Select(d => d.id)
                .ToArray();
            
        }

        public override void OnUnload()
        {
            EventManager.onCreatureKill -= EventManager_onCreatureKill;
            EventManager.onPossess -= EventManager_onPossess;
            EventManager.onUnpossess -= EventManager_onUnpossess;
        }

        private void EventManager_onCreatureKill(Creature creature, Player player, CollisionInstance collisionInstance,
            EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart || player || !collisionInstance.IsDoneByPlayer())
                return;
            if(idx == itemIds.Length - 1 )
                return;

            Arm();
            idx++;
            idx = Mathf.Clamp(idx,0, itemIds.Length-1);
        }

        private void EventManager_onPossess(Creature creature, EventTime eventTime)
        {
            if (eventTime == EventTime.OnEnd)
            {

                creature.handLeft.OnGrabEvent += OnGrabEvent;
                creature.handRight.OnGrabEvent += OnGrabEvent;
            }
            Arm();

            DisplayText.ShowText(new DisplayText.TextPriority($"Welcome to Blade Game! Kill to level up your weapon!", 10,
                TutorialData.TextType.INFORMATION,
                6f));
        }

        private void EventManager_onUnpossess(Creature creature, EventTime eventTime)
        {
            creature.handLeft.OnGrabEvent -= OnGrabEvent;
            creature.handRight.OnGrabEvent -= OnGrabEvent;
        }

        private void OnGrabEvent(Side side, Handle handle, float axisPosition, HandleOrientation orientation, EventTime eventTime)
        {
            var item = handle.item;
            if (allowStealing || item == null)
                return;

            foreach (var itemType in allowedItemTypes)
                if (item.data.type == itemType)
                    return;
            //If the player picked up a item they are not on right now, make em drop it
            if (item.data.id != itemIds[idx]) Level.current.StartCoroutine(DisarmCoroutine(handle));
        }

        private IEnumerator DisarmCoroutine(Handle handle)
        {
            yield return waitFor0_5Second;
            Disarm(handle);
        }

        private void Disarm(Handle handle)
        {
            if (handle != null)
                for (var i = handle.handlers.Count - 1; i >= 0; --i)
                    handle.handlers[i].UnGrab(false);
        }

        private void Arm()
        {
            if (Player.currentCreature == null)
                return;

            var hand = Player.currentCreature.handRight;
            if (GameManager.options.twoHandedDominantHand == Side.Left) hand = Player.currentCreature.handLeft;

            Level.current.StartCoroutine(ArmCoroutine(hand));
        }

        private IEnumerator ArmCoroutine(RagdollHand hand)
        {
            yield return DisarmCoroutine(hand.grabbedHandle);
            yield return waitFor0_5Second;

            Catalog.GetData<ItemData>(itemIds[idx])?.SpawnAsync(item => {
                Disarm(hand.grabbedHandle);
                hand.Grab(item.GetMainHandle(GameManager.options.twoHandedDominantHand), true);
                rewardFxData?.Spawn(hand.transform.position, hand.transform.rotation).Play();
            });
        }
    }
}
