using System.Collections;
using ThunderRoad;
using Wully.MoreModes;
using Wully.Utils;

namespace Wully.MoreModes {
	public class NoTeleGrabRagdoll : ModifierData {
		
		public static NoTeleGrabRagdoll Instance;
        
		public override void Init()
		{
			if (Instance == null)
			{
				base.Init();
				Instance = this;
				local = this;
			}
		}
		
		public override void Update()
		{
			base.Update();
			if (Player.currentCreature)
			{
				Player.currentCreature.mana.casterLeft.telekinesis.grabRagdoll = false;
				Player.currentCreature.mana.casterRight.telekinesis.grabRagdoll = false;
			}
		}
	}
}