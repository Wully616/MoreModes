using System.Collections;

using ThunderRoad;
using UnityEngine;
using Wully.MoreModes;
using Wully.Utils;

namespace Wully.MoreModes {
	public class RipAndTear : ModifierData {
		public static RipAndTear Instance;


		public override void Init()
		{
			if (Instance != null) return;
			
			base.Init();
			local = Instance = this;
		}
		
		protected override void OnEnable()
		{
			base.OnEnable();
			EventManager.onCreatureSpawn += OnCreatureSpawn;
			int allActiveCount = Creature.allActive.Count;
			for (var i = 0; i < allActiveCount; i++)
			{
				Creature creature = Creature.allActive[i];
				EnableTearing(creature);
			}
		}
		private static void EnableTearing(Creature creature)
		{
			creature.ragdoll.headPart.ripBreak = true;
			int partsCount = creature.ragdoll.parts.Count;
			for (var j = 0; j < partsCount; j++)
			{
				RagdollPart ragdollPart = creature.ragdoll.parts[j];
				ragdollPart.EnableCharJointBreakForce(0.75f); //TODO: get good value for this
			}
		}
		
		private static void DisableTearing(Creature creature)
		{
			creature.ragdoll.headPart.ripBreak = false;
			int partsCount = creature.ragdoll.parts.Count;
			for (var j = 0; j < partsCount; j++)
			{
				RagdollPart ragdollPart = creature.ragdoll.parts[j];
				ragdollPart.ResetCharJointBreakForce();
			}
		}

		private void OnCreatureSpawn(Creature creature)
		{
			EnableTearing(creature);
		}


		protected override void OnDisable()
		{
			base.OnDisable();
			EventManager.onCreatureSpawn -= OnCreatureSpawn;
			int allActiveCount = Creature.allActive.Count;
			for (var i = 0; i < allActiveCount; i++)
			{
				Creature creature = Creature.allActive[i];
				DisableTearing(creature);
			}
		}
		
	}
}