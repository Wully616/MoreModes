using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameModeLoader.Utils;
using Sirenix.Utilities;
using ThunderRoad;
using UnityEngine;
using Wully.MoreModes;

namespace Wully.MoreModes.GameMode
{
    /// <summary>
    ///     This survival mode inherits from the base games survival game mode
    /// </summary>
    public class BladeGame : LevelModule
    {
        public bool anyKillCounts = false;
        public float startDelay = 10f;
        public List<string> excludeItemIds;
        public List<string> tierWaves;
        public bool allowStealing = false;
        private WaitForSeconds waitFor0_5Second = new WaitForSeconds(0.5f);
        private int idx = 0;
        private int currentTier = 0;
        private string[] itemIds;

        private EffectData rewardFxData;
        private bool complete;
        public string rewardFxId = "SurvivalMode.RewardFx";
        protected Coroutine waveEndedCoroutine;

        protected WaveSpawner waveSpawner;

        private List<ItemData.Type> allowedItemTypes = new List<ItemData.Type> {
            ItemData.Type.Prop,
            ItemData.Type.Body,
            ItemData.Type.Misc,
            ItemData.Type.Wardrobe,
            ItemData.Type.Potion
        };

        public override IEnumerator OnLoadCoroutine()
        {
            EventManager.onCreatureKill += EventManager_onCreatureKill;
            EventManager.onPossess += EventManager_onPossess;
            EventManager.onUnpossess += EventManager_onUnpossess;
            rewardFxData = Catalog.GetData<EffectData>(rewardFxId);
            System.Random rnd = new System.Random();

            List<ItemData> itemData = Catalog.GetDataList(Catalog.Category.Item)
                .Cast<ItemData>()
                .Where(d => d.type == ItemData.Type.Weapon && d.mass >= 0.1f && d.purchasable == true && d.slot != "Arrow" && d.slot != "Bow" && d.damagers.Count > 0 && !(d.damagers.Count == 1 && d.damagers[0].damagerID == "Handle1H") && !excludeItemIds.Contains(d.id))
                .ToList();

            // get each tier and randomise them
            itemIds = itemData
                .Where(d => d.tier == 0).OrderBy(d => rnd.Next()).Select(d => d.id).ToArray()
                .Concat(itemData.Where(d => d.tier == 1).OrderBy(d => rnd.Next()).Select(d => d.id).ToArray())
                .Concat(itemData.Where(d => d.tier == 2).OrderBy(d => rnd.Next()).Select(d => d.id).ToArray())
                .Concat(itemData.Where(d => d.tier == 3).OrderBy(d => rnd.Next()).Select(d => d.id).ToArray())
                .ToArray();


            Debug.Log(string.Join(", ", itemIds));
            if (WaveSpawner.instances.Count > 0)
            {
                waveSpawner = WaveSpawner.instances[0];
                waveSpawner.OnWaveAnyEndEvent.AddListener(OnWaveEnded);
                level.StartCoroutine(LevelLoadedCoroutine());
            }
            yield break;
        }

        protected void OnWaveEnded()
        {
            waveEndedCoroutine = level.StartCoroutine(WaveEndedCoroutine());
        }

        private IEnumerator WaveEndedCoroutine()
        {
            yield return new WaitForSeconds(2f);
            Utilities.Message($"Tier {currentTier} complete!: {tierWaves[currentTier]}");
        }


        private IEnumerator LevelLoadedCoroutine()
        {
            while (!Player.local || !Player.local.creature)
                yield return new WaitForSeconds(2f);


            yield return new WaitForSeconds(startDelay);
            for (int i = 3; i > 0; --i)
            {
                Utilities.Message($"{i}");
                yield return new WaitForSeconds(2f);
            }

            Utilities.Message($"Starting Tier {currentTier} wave: {tierWaves[currentTier]}");
            
            yield return new WaitForSeconds(1f);
            WaveData data = Catalog.GetData<WaveData>(tierWaves[currentTier]);
            if (data != null)
                waveSpawner.StartWave(data, 5f);
            else
                Debug.LogError("Unable to find wave " + tierWaves[currentTier]);
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
            if (eventTime == EventTime.OnStart) return;
            if (player) return;

            if (!anyKillCounts && !collisionInstance.IsDoneByPlayer())
                return;

            //only arm if there is a new index to go to, once were at the end, no more arming
            if (idx != itemIds.Length - 1)
            {
                Arm();
                idx++;
                idx = Mathf.Clamp(idx, 0, itemIds.Length - 1);
            }
            else
            {
                if (!complete)
                {
                    allowStealing = true;
                    Utilities.Message($"You killed with the final weapon! You have completed Blade Game with {idx} weapons!");
                    complete = true;
                }
                return;
            }
        }

        private void EventManager_onPossess(Creature creature, EventTime eventTime)
        {
            if (eventTime == EventTime.OnEnd)
            {

                creature.handLeft.OnGrabEvent += OnGrabEvent;
                creature.handRight.OnGrabEvent += OnGrabEvent;
            }
            if (idx == 0)
            {
                Utilities.Message($"Welcome to Blade Game! Kill to level up your weapon!");
            }
            Arm();
        }

        private void EventManager_onUnpossess(Creature creature, EventTime eventTime)
        {
            creature.handLeft.OnGrabEvent -= OnGrabEvent;
            creature.handRight.OnGrabEvent -= OnGrabEvent;
        }

        private void OnGrabEvent(Side side, Handle handle, float axisPosition, HandlePose orientation, EventTime eventTime)
        {
            var item = handle.item;
            if (allowStealing || item == null)
                return;

            foreach (var itemType in allowedItemTypes)
                if (item.data.type == itemType)
                    return;
            //If the player picked up a item they are not on right now, make em drop it
            if (idx == -1 || item.data.id != itemIds[idx]) Level.current.StartCoroutine(DisarmCoroutine());
        }

        private IEnumerator DisarmCoroutine()
        {
            yield return waitFor0_5Second;
            Disarm();
        }

        private void Disarm()
        {
            Player.local.creature.handLeft.TryRelease();
            Player.local.creature.handRight.TryRelease();
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
            Catalog.GetData<ItemData>(itemIds[idx])?.SpawnAsync(item => {
                //if the weapon spawned is of the next tier, stop the wave and spawn the next tier
                if (item.data.tier != currentTier)
                {
                    currentTier++;
                    waveSpawner.StopWave(true);
                    level.StartCoroutine(LevelLoadedCoroutine());
                }
                Disarm();
                hand.Grab(item.GetMainHandle(GameManager.options.twoHandedDominantHand), true);
                Transform handTransform = hand.transform;
                rewardFxData?.Spawn(handTransform.position, handTransform.rotation).Play();
            });
            yield break;
        }
    }
}
