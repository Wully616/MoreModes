using System.Collections;
using ThunderRoad;
using UnityEngine;
using Wully.MoreModes;
using Wully.Utils;

namespace Wully.MoreModes {
	public class MightyKick : ModifierData {
		//TODO: doesnt really work as hoped
		public static MightyKick Instance;
        
		public override void Init()
		{
			if (Instance == null)
			{
				base.Init();
				Instance = this;
				local = this;
			}
		}
		
		protected override void OnEnable()
		{
			base.OnEnable();
			foreach (var damager in Player.local.footLeft.ragdollFoot.collisionHandler.damagers)
			{
				damager.data.addForce *= 100;
				damager.data.addForceRagdollOtherMultiplier *= 2f;
				damager.data.addForceMode = ForceMode.VelocityChange;
			}
			foreach (var damager in Player.local.footRight.ragdollFoot.collisionHandler.damagers)
			{
				damager.data.addForce *= 100;
				damager.data.addForceRagdollOtherMultiplier *= 2f;
				damager.data.addForceMode = ForceMode.VelocityChange;
			}

		}
		
		protected override void OnDisable() {
			base.OnDisable();
			foreach (var damager in Player.local.footLeft.ragdollFoot.collisionHandler.damagers)
			{
				damager.data.addForce /= 100;
				damager.data.addForceRagdollOtherMultiplier /= 2f;
				damager.data.addForceMode = ForceMode.Acceleration;
			}
			foreach (var damager in Player.local.footRight.ragdollFoot.collisionHandler.damagers)
			{
				damager.data.addForce /= 100;
				damager.data.addForceRagdollOtherMultiplier /= 2f;
				damager.data.addForceMode = ForceMode.Acceleration;
			}

		}
		
	}
}