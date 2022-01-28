using System.Collections;
using GameModeLoader.Data;
using GameModeLoader.Utils;
using ThunderRoad;
using Wully.Utils;
using UnityEngine;

namespace GameModeLoader.Component
{
    public class LunarGravity : LevelModuleOptional
	{

		/// <summary>
		/// You should always have an OnLoadCoroutine
		/// </summary>
		public override IEnumerator OnLoadCoroutine()
		{
			//You must always call the following, so the IDs are setup for this LevelModuleOptional
			SetId();

			//Use IsEnabled around functionality so it only runs if this LevelModuleOptional is enabled
			if (IsEnabled())
			{
				Debug.Log($"This levelModuleOption is enabled - {id}");
				Utilities.LunarGravity();
			}

			yield break;
		}

		public override void Update()
		{
			base.Update();

			//Update runs all the time, so make sure you wrap your logic in IsEnabled
			if (IsEnabled())
			{
				///Debug.Log($"This levelModuleOption is updating - {id}");
			}
		}

		public override void OnUnload()
		{
			base.OnUnload();
			//Remember to unsubscribe to any events you might be listening to
			if (IsEnabled())
			{
				Debug.Log($"This levelModuleOption is unloaded - {id}");
			}

		}
	}
}