using System.Collections;
using ThunderRoad;
using Wully.MoreModes.Data;
using Wully.Utils;

namespace Wully.MoreModes {
	public class FreeClimb : Modifier {
		
		public static FreeClimb Instance;
        
		public override void Init()
		{
			if (Instance == null)
			{
				base.Init();
				Instance = this;
				// bit hacky, but we only one 1 modifier, if local isnt set, this modifier isnt Setup
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