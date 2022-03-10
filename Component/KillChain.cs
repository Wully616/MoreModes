using System.Collections;
using GameModeLoader.Data;
using ThunderRoad;
using UnityEngine;

namespace GameModeLoader.Component
{
    public class KillChain : LevelModuleOptional
    {
        public float timeBetweenKill = 15f;
        private float lastKillTime;
        private bool firstKill;

        private int kills = 0;
        private bool win = true;
        private EffectData rewardFxData;
        public string rewardFxId = "KillChain.RewardFx";
        protected WaveSpawner waveSpawner;

        public override IEnumerator OnLoadCoroutine()
        {
            SetId();
            if (!IsEnabled()) yield break;
            EventManager.onCreatureKill += EventManager_onCreatureKill;

            rewardFxData = Catalog.GetData<EffectData>(rewardFxId);
            if (rewardFxData == null)
            {
                Debug.LogWarning($"KillChain reward FX {rewardFxId} is missing");
            }
            if ( WaveSpawner.instances.Count > 0 )
            {
                waveSpawner = WaveSpawner.instances[0];
                waveSpawner.OnWaveEndEvent.AddListener(OnWaveEnd);

            }

            level.StartCoroutine(LevelLoadedCoroutine());
        }
        private IEnumerator LevelLoadedCoroutine()
        {
            while ( !Player.local || !Player.local.creature )
                yield return new WaitForSeconds(2f);


            DisplayText.ShowText(new DisplayText.TextPriority($"Welcome to Kill Chain! Keep killing to stay alive using ANY means, you have {timeBetweenKill} seconds to kill your next target. The first timer will start after your first kill.", 10,
                TutorialData.TextType.SUBTITLE,
                6f));
        }

        public override void OnUnload()
        {
            EventManager.onCreatureKill -= EventManager_onCreatureKill;
        }

        private void OnWaveEnd()
        {
            string text = win ? "WIN" : "FAILED";
            DisplayText.ShowText(new DisplayText.TextPriority($"You {text}! You got {kills} kills. Please start a new wave to restart", 9,
                TutorialData.TextType.SUBTITLE,
                6f));
            firstKill = false;
            win = true;
        }

        public override void Update()
        {
            if (firstKill)
            {
                if (Time.time >= lastKillTime + timeBetweenKill)
                {
                    Fail();
                }
            }
            
        }

        private void EventManager_onCreatureKill(Creature creature, Player player, CollisionInstance collisionInstance,
            EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart) return;
            if (player)
            {
                Fail();
                return;
            }

            lastKillTime = Time.time;
            rewardFxData?.Spawn(Player.local.locomotion.transform.position, Quaternion.identity).Play();
            //Player did their first till, start the timer
            if (!firstKill)
            {
                firstKill = true;
                kills = 1;
            }
            else
            {
                kills++;
            }

        }


        private void Fail()
        {
            win = false;
            waveSpawner.StopWave(false);
            firstKill = false;

        }
    }
}
