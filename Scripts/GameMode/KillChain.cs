using System.Collections;
using GameModeLoader.Utils;
using ThunderRoad;
using UnityEngine;
using Wully.MoreModes;

namespace Wully.MoreModes.GameMode
{
    public class KillChain : LevelModule
    {
        public float timeBetweenKill = 20f;
        private float timeMultiplier = 10f;
        private float lastKillTime;
        private bool firstKill;

        private int kills = 0;
        private bool win = true;
        private EffectData rewardFxData;
        public string rewardFxId = "KillChain.RewardFx";
        protected WaveSpawner waveSpawner;

        public override IEnumerator OnLoadCoroutine()
        {
            EventManager.onCreatureKill += EventManager_onCreatureKill;

            rewardFxData = Catalog.GetData<EffectData>(rewardFxId);
            if (rewardFxData == null)
            {
                Debug.LogWarning($"KillChain reward FX {rewardFxId} is missing");
            }
            
            if(level.options.TryGetValue("difficulty", out string difficulty) && float.TryParse(difficulty, out float value))
            {
                timeBetweenKill = 60f - (value * timeMultiplier);
                Debug.Log($"KilLChain Difficulty: {difficulty}. Time between kills: {timeBetweenKill}");
            }
                
            if ( WaveSpawner.instances.Count > 0 )
            {
                waveSpawner = WaveSpawner.instances[0];
                waveSpawner.OnWaveAnyEndEvent.AddListener(OnWaveEnd);

            }
            
            
            level.StartCoroutine(LevelLoadedCoroutine());
            yield break;
        }
        private IEnumerator LevelLoadedCoroutine()
        {
            while ( !Player.local || !Player.local.creature )
                yield return new WaitForSeconds(2f);
            
            yield return new WaitForSeconds(2f);
            
            Utilities.Message($"Welcome to Kill Chain! Keep killing to stay alive using ANY means, you have {timeBetweenKill} seconds to kill your next target. The first timer will start after your first kill.", 10, true, 6f);
        }

        public override void OnUnload()
        {
            EventManager.onCreatureKill -= EventManager_onCreatureKill;
        }

        private void OnWaveEnd()
        {
            string text = win ? "WIN" : "FAILED";
            Utilities.Message($"You {text}! You got {kills} kills. Please start a new wave to restart");
            firstKill = false;
            win = true;
        }
        private Coroutine counter;
        private TextMesh textMesh;
        public override void Update()
        {
            if (firstKill)
            {
                if (counter == null)
                {
                    textMesh = ShowCount((int)timeBetweenKill);
                    counter = Level.current.StartCoroutine(StartCounter(textMesh));
                }
                if (textMesh)
                {
                    var textMeshTransform = textMesh.transform;
                    Transform headTransform = Player.local.head.transform;
                    Vector3 headTransformPosition = headTransform.position;
                    textMeshTransform.rotation = Quaternion.LookRotation(textMeshTransform.position - headTransformPosition);
                    textMeshTransform.position = headTransformPosition + (headTransform.forward * 2f) + headTransform.right;
                }
                if (Time.time >= lastKillTime + timeBetweenKill)
                {
                    Fail();
                }
            }
            
        }

        private IEnumerator StartCounter(TextMesh textMesh)
        {
            while (firstKill)
            {
                int left = (int)((timeBetweenKill + lastKillTime) - (Time.time));
                textMesh.text = left.ToString();
                textMesh.color = Color.Lerp(Color.red, Color.green, left / timeBetweenKill);
                yield return Yielders.ForSeconds(1f);
            }
            GameObject.Destroy(textMesh.gameObject);
            
        }
        
        private TextMesh ShowCount(int time)
        {
            var holder = new GameObject();
            var text = holder.AddComponent<TextMesh>();
            text.text = time.ToString();
            text.anchor = TextAnchor.MiddleCenter;
            Transform headTransform = Player.local.head.transform;
            text.transform.position = headTransform.position + (headTransform.forward * 2f) + headTransform.right;
            text.characterSize = 0.03f;
            text.fontSize = 25;
            text.color = Color.Lerp(Color.red, Color.green, time / timeBetweenKill);
            return text;

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
            Level.current.StopCoroutine(counter);
            if (textMesh)
            {
                GameObject.Destroy(textMesh.gameObject);
            }
        }
    }
}
