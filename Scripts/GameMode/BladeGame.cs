using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameModeLoader.Utils;

using ThunderRoad;
using UnityEngine;
using Wully.Utils;

namespace Wully.MoreModes.GameMode
{
    public class BladeGame : LevelModule
    {
        public bool anyKillCounts = false;
        public bool reverseOrderWeapons;
        public bool randomizeOrder;
        
        public float startDelay = 10f;
        public List<string> excludeItemIds;
        public List<string> tierWaves;
        public bool allowStealing = false;
        private int idx = 0;
        private int currentTier = 0;
        private string[] itemIds;

        private EffectData rewardFxData;
        private bool complete;
        public string rewardFxId = "KillChain.RewardFx";
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

            anyKillCounts = level.GetOptionAsBool("anykill", false);
            allowStealing = level.GetOptionAsBool("allowsteal", false);
            reverseOrderWeapons = level.GetOptionAsBool("reverseOrder", false);
            randomizeOrder = level.GetOptionAsBool("randomizeOrder", false);
            
            System.Random rnd = new System.Random();

            List<ItemData> itemData = Catalog.GetDataList(Catalog.Category.Item)
                .Cast<ItemData>()
                .Where(d => d.type == ItemData.Type.Weapon && d.mass >= 0.1f && d.slot != "Arrow" && d.slot != "Bow" && d.damagers.Count > 0 && !(d.damagers.Count == 1 && d.damagers[0].damagerID == "Handle1H") && !excludeItemIds.Contains(d.id))
                .ToList();

            // get each tier and randomise them
            if (randomizeOrder)
            {
                itemIds = itemData.OrderBy(d => rnd.Next()).Select(d => d.id).ToArray();
            }
            else
            {
                itemIds = itemData
                    .Where(d => d.tier == 0).OrderBy(d => rnd.Next()).Select(d => d.id).ToArray()
                    .Concat(itemData.Where(d => d.tier == 1).OrderBy(d => rnd.Next()).Select(d => d.id).ToArray())
                    .Concat(itemData.Where(d => d.tier == 2).OrderBy(d => rnd.Next()).Select(d => d.id).ToArray())
                    .Concat(itemData.Where(d => d.tier == 3).OrderBy(d => rnd.Next()).Select(d => d.id).ToArray())
                    .ToArray();
            }
            
            if (reverseOrderWeapons)
            {
                itemIds = itemIds.Reverse().ToArray();
            }


            Debug.Log(string.Join(", ", itemIds));
            if (!level.dungeon)
            {
                if (WaveSpawner.instances.Count > 0)
                {
                    waveSpawner = WaveSpawner.instances[0];
                    waveSpawner.OnWaveAnyEndEvent.AddListener(OnWaveEnded);
                    level.StartCoroutine(LevelLoadedCoroutine());
                }
            }
            yield break;
        }

        protected void OnWaveEnded()
        {
            waveEndedCoroutine = level.StartCoroutine(WaveEndedCoroutine());
        }

        private IEnumerator WaveEndedCoroutine()
        {
            yield return Yielders.ForSeconds(2f);
            Utilities.Message($"Tier {currentTier} complete!: {tierWaves[currentTier]}");
        }


        private IEnumerator LevelLoadedCoroutine()
        {
            while (!Player.local || !Player.local.creature)
                yield return Yielders.ForSeconds(2f);


            yield return new WaitForSeconds(startDelay);
            for (int i = 3; i > 0; --i)
            {
                Utilities.Message($"{i}");
                yield return Yielders.ForSeconds(2f);
            }

            Utilities.Message($"Starting Tier {currentTier} wave: {tierWaves[currentTier]}");
            
            yield return Yielders.ForSeconds(1f);
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

            if (!anyKillCounts && !collisionInstance.IsDoneByPlayer()) return;
            //only arm if there is a new index to go to, once were at the end, no more arming
            if (idx != itemIds.Length - 1)
            {
                idx++;
                idx = Mathf.Clamp(idx, 0, itemIds.Length - 1);
                Arm();
            }
            else
            {
                if (!complete)
                {
                    allowStealing = true;
                    Utilities.Message($"You killed with the final weapon! You have completed Blade Game with {idx} weapons!");
                    complete = true;
                }
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
            if(eventTime == EventTime.OnStart) return;
            var item = handle.item;
            if (allowStealing || item == null)
                return;

            if (item.data.type == ItemData.Type.Weapon)
            {
                //If the player picked up a item they are not on right now, make em drop it
                if (idx == -1 || item.data.id != itemIds[idx])
                {
                    if(side == Side.Left) Player.local.creature.handLeft.TryRelease();
                    if(side == Side.Right) Player.local.creature.handRight.TryRelease();
                }     
            }
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

            Catalog.GetData<ItemData>(itemIds[idx])?.SpawnAsync(item => {
                //if the weapon spawned is of the next tier, stop the wave and spawn the next tier
                if (item.data.tier != currentTier)
                {
                    currentTier++;
                    if (!level.dungeon)
                    {
                        waveSpawner.StopWave(true);
                        level.StartCoroutine(LevelLoadedCoroutine());
                    }
                }                
                Disarm();
                hand.Grab(item.GetMainHandle(GameManager.options.twoHandedDominantHand), true);
                Transform handTransform = hand.transform;
                rewardFxData?.Spawn(handTransform.position, handTransform.rotation).Play();
            });
        }
        
    }
}
