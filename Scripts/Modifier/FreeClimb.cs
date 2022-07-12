using System.Collections;
using ThunderRoad;
using Wully.MoreModes.Data;
using Wully.Utils;

namespace Wully.MoreModes {
	public class FreeClimb : Modifier {
		
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