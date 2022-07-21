using System.Collections;
using ThunderRoad;
using Wully.MoreModes;
using Wully.Utils;

namespace Wully.MoreModes {
	public class FreeClimb : ModifierData {
		
		public static FreeClimb Instance;
        
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
			RagdollHandClimb.climbFree = true;
		}
		
		protected override void OnDisable() {
			base.OnDisable();
			RagdollHandClimb.climbFree = false;
		}
		
	}
}