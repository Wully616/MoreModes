using System.Collections;
using ThunderRoad;
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
			Player.local.footLeft.kickExtendDuration = 0.001f;
			Player.local.footRight.kickExtendDuration = 0.001f;
		}
		
		protected override void OnDisable() {
			base.OnDisable();
			Player.local.footLeft.kickExtendDuration = 0.2f;
			Player.local.footRight.kickExtendDuration = 0.2f;
		}
		
	}
}