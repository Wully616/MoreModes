using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameModeLoader.Utils;

using ThunderRoad;
using UnityEngine;
using UnityEngine.AI;
using Wully.Utils;
using Random = UnityEngine.Random;

namespace Wully.MoreModes.GameMode
{
    public class TargetPractice : LevelModule
    {

        public string targetPropId = "ArcheryTarget";
        private ItemData targetItemData;
        private EffectData rewardFxData;
        public string rewardFxId = "KillChain.RewardFx";
        private float startDelay = 4f;

        private float time;
        private float score;
        private bool first;

        private int numberOfTargets = 10;
        private int targetDistance = 10;
        
        public override IEnumerator OnLoadCoroutine()
        {
            
            rewardFxData = Catalog.GetData<EffectData>(rewardFxId);

            targetItemData = Catalog.GetData<ItemData>(targetPropId);
            
            if (targetItemData != null && !level.dungeon)
            {
                level.StartCoroutine(LevelLoadedCoroutine());
            }
            
            yield break;
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

            Utilities.Message($"Welcome to Target practice. Targets will appear along your path. Try to shoot, hit, fireball them as fast as you can! Timer starts after first hit", 10, false);
            
            yield return Yielders.ForSeconds(1f);
            first = true;
            SpawnTarget();
        }

        public virtual void SpawnTarget()
        {
            if (!Player.local && numberOfTargets == 0) return;
            if (first) time = DateTime.UtcNow.Millisecond;
            first = false;
            
            targetItemData.SpawnAsync(item => {
                item.rb.isKinematic = true;
                item.transform.position = GetSpawnPosition();
                while (!item.renderers[0].isVisible)
                {
                    item.transform.position = GetSpawnPosition();
                }
                item.transform.LookAt(Player.local.locomotion.transform);
                
                item.mainCollisionHandler.OnCollisionStartEvent += (instance) => {
                    OnHitTarget(instance, item);
                };
            });
        }

        public Vector3 GetSpawnPosition()
        {
            //get a random angle in about 100 degrees
            Quaternion randAng = Quaternion.Euler(0, Random.Range(-90,90), 0);
            //get random angle + player forward direction
            var direction = randAng * Player.local.locomotion.transform.forward;
            direction *= targetDistance;
            
            //get a position around the player in a unit sphere, find valid position on navmesh
            var pos = Player.local.locomotion.transform.position;
            direction.x += pos.x;
            direction.y = pos.y;
            direction.z += pos.z;

            NavMeshHit hit;
            NavMesh.SamplePosition(direction, out hit, 1f, 1);
            Vector3 hitPos = hit.position;
            hitPos.y = Player.local.head.transform.position.y;
            return hitPos;
        }
        private void OnHitTarget(CollisionInstance collisioninstance, Item item)
        {
            Debug.Log($"Was hit by {collisioninstance.sourceCollider.gameObject.name}");
            
            if (!collisioninstance.IsDoneByPlayer()) return;

            numberOfTargets -= 1;
            Debug.Log($"Target hit. Targets left {numberOfTargets}");
            rewardFxData.Spawn(item.transform.position, Quaternion.identity);
            if (numberOfTargets > 0)
            {
                SpawnTarget();
                
            }
            else
            {
                TimeSpan timeSpan = TimeSpan.FromMilliseconds(time - DateTime.UtcNow.Millisecond);
                Utilities.Message($"Well done you hit all the targets! You did it in {timeSpan.ToString()}", 10, false);
            }
            item.Despawn();
            
        }
    }
    
}
