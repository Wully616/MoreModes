using ThunderRoad;

namespace Wully.MoreModes {
	public class StopBullets : ModifierData {
		
		public static StopBullets Instance;
		
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
		}
		
		
		protected override void OnDisable() {
			base.OnDisable();
		
		}

		public override void Update()
		{
			base.Update();
		}
	}
}