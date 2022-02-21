using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameModeLoader.Data;
using Sirenix.Utilities;
using ThunderRoad;
using UnityEngine;

namespace GameModeLoader.Component
{
    /// <summary>
    ///     This is a special levelModuleOption which is always enabled the master level
    ///     and it will remove the base games Cleaner from maps
    /// </summary>
    public class Cleaner : LevelModule
    {
        private static List<Creature> deadCreatures;
        protected bool cleanBodies;
        public float cleanerRate = 5f;
        protected float lastCleaningTime;
        public override IEnumerator OnLoadCoroutine()
        {
            Debug.Log("Cleaner activated");
			EventManager.onLevelLoad += EventManager_onLevelLoad;
            yield break;
        }

		private void EventManager_onLevelLoad( LevelData levelData, EventTime eventTime )
		{
            if(eventTime == EventTime.OnEnd) return;
            if ( levelData.id.ToLower() == "master" ) return;
            //Unload Cleaner
            Debug.Log($"Cleaner removing LevelModuleCleaner from {levelData.id}");
            foreach (LevelData.Mode levelDataMode in levelData.modes)
            {
                var levelModuleCleaner = levelDataMode.modules?.FirstOrDefault(d => d.type == typeof(LevelModuleCleaner));
                if ( levelModuleCleaner == null ) continue;

                //call unload
                levelModuleCleaner.OnUnload();
				//remove
                if (!levelDataMode.modules.IsNullOrEmpty())
                {
                    for (var i = levelDataMode.modules.Count - 1; i >= 0; i--)
                        if ( levelDataMode.modules[i] == levelModuleCleaner)
                            levelDataMode.modules.RemoveAt(i);
                }
            }

        }


        public override void Update()
        {

            if (lastCleaningTime > cleanerRate)
            {
                if (cleanBodies)
                {
                    CleanDeadBodies();
                    cleanBodies = false;
                }
                else
                {
                    CleanDroppedObjects();
                    cleanBodies = true;
                }
                lastCleaningTime = 0.0f;
            }
            lastCleaningTime += Time.deltaTime;
        }

        public void CleanDroppedObjects()
        {
            var items = Item.allActive
                .Where(d => !d.holder && !d.isTelekinesisGrabbed && !d.isThrowed && !d.isGripped && !d.IsHanded() && d.spawnTime != 0.0 && !d.disallowDespawn)
                .OrderBy(d => d.lastInteractionTime).ToList();

            if (items.Count <= 0)
                return;

            //only keep unique items.
            var toRemove = items
                .GroupBy(x => x.data.id)
                .SelectMany(g => g.Skip(1));

            //Remove the duplicate items
            var itemToRemove = toRemove.GetEnumerator();
            while (itemToRemove.MoveNext())
                if (Time.time - itemToRemove?.Current?.lastInteractionTime >= Catalog.gameData.cleanObjectsLastInteractionDelay)
                    itemToRemove.Current.Despawn();
            itemToRemove.Dispose();

            //Remove anything else that might be more than the max drop count
            var array = Item.allActive.Where(d => !d.holder && !d.isTelekinesisGrabbed && !d.isThrowed && !d.isGripped && !d.IsHanded() && d.spawnTime != 0.0 && !d.disallowDespawn).OrderBy(d => d.lastInteractionTime).ToArray();
            var num = array.Length - LevelModuleCleaner.cleanMaxDropCount;
            if (num <= 0)
                return;
            for (var index = 0; index < num && Time.time - array[index].lastInteractionTime >= Catalog.gameData.cleanObjectsLastInteractionDelay; ++index)
                if (!array[index].IsVisible())
                    array[index].Despawn();
        }

        public void CleanDeadBodies()
        {
            var allActive = Creature.allActive;
            if (deadCreatures == null)
                deadCreatures = new List<Creature>(5);
            deadCreatures.Clear();
            for (var index = 0; index < allActive.Count; ++index)
            {
                var creature = allActive[index];
                if (creature.state == Creature.State.Dead && !creature.ragdoll.isGrabbed && !creature.ragdoll.isTkGrabbed)
                    deadCreatures.Add(creature);
            }
            var num = deadCreatures.Count - LevelModuleCleaner.cleanMaxDeadCount;
            if (num <= 0)
                return;
            deadCreatures.Sort(Creature.CompareByLastInteractionTime);
            for (var index = 0; index < num && Time.time - deadCreatures[index].lastInteractionTime >= Catalog.gameData.cleanDeadLastInteractionDelay; ++index)
                deadCreatures[index].Despawn();
        }
    }
}
