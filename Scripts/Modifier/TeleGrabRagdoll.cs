using System.Collections;
using ThunderRoad;
using Wully.MoreModes;
using Wully.Utils;

namespace Wully.MoreModes {
	public class TeleGrabRagdoll : ModifierData {
		
		public static TeleGrabRagdoll Instance;
        
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
				Player.currentCreature.mana.casterLeft.telekinesis.grabRagdoll = true;
				Player.currentCreature.mana.casterRight.telekinesis.grabRagdoll = true;
			}
		}
	}
}