using System.Collections;
using GameModeLoader.Data;
using ThunderRoad;
using UnityEngine;
using Wully.Utils;

namespace GameModeLoader.Component
{
    /// <summary>
    /// Starts a particular wave on level start
    /// </summary>
    public class StartWave : LevelModuleOptional
    {
        public float startDelay = 10f;
        public string waveId = "Bandit1";
        protected Coroutine waveEndedCoroutine;

        protected WaveSpawner waveSpawner;
        /// <summary>
        /// You should always have an OnLoadCoroutine
        /// </summary>
        public override IEnumerator OnLoadCoroutine()
        {
            SetId();
            if (!IsEnabled())
            {
                yield break;
            }

            if (WaveSpawner.instances.Count > 0)
            {
                waveSpawner = WaveSpawner.instances[0];
                waveSpawner.OnWaveAnyEndEvent.AddListener(OnWaveEnded);
                level.StartCoroutine(LevelLoadedCoroutine());
                yield break;
            }

            yield break;
        }

        protected new void OnWaveEnded()
        {
            waveEndedCoroutine = level.StartCoroutine(WaveEndedCoroutine());
        }

        private IEnumerator WaveEndedCoroutine()
        {
            yield return new WaitForSeconds(2f);
            DisplayMessage.ShowMessage(new DisplayMessage.MessageData("Wave complete!: " + waveId,
                10, DisplayMessage.TextType.INFORMATION, 3f));
        }

        private IEnumerator LevelLoadedCoroutine()
        {
            while (!Player.local || !Player.local.creature)
                yield return new WaitForSeconds(2f);


            yield return new WaitForSeconds(startDelay);
            for (int i = 3; i > 0; --i)
            {
                DisplayMessage.ShowMessage(new DisplayMessage.MessageData(i.ToString(), 10, DisplayMessage.TextType.INFORMATION,
                    1f));
                yield return new WaitForSeconds(2f);
            }

            DisplayMessage.ShowMessage(new DisplayMessage.MessageData("Starting wave: " + waveId,
                10, DisplayMessage.TextType.INFORMATION, 3f));
            yield return new WaitForSeconds(1f);
            WaveData data = Catalog.GetData<WaveData>(waveId);
            if (data != null)
                waveSpawner.StartWave(data, 5f);
            else
                Debug.LogError("Unable to find wave " + waveId);
        }


        public override void OnUnload()
        {
            base.OnUnload();
            if (WaveSpawner.instances.Count > 0)
            {
                waveSpawner = WaveSpawner.instances[0];
                waveSpawner.OnWaveAnyEndEvent.RemoveListener(OnWaveEnded);
            }

        }
    }
}
