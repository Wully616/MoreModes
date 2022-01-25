using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using GameModeLoader.Module;
using ThunderRoad;
using Wully.Utils;

namespace GameModeLoader.Data {
	
	public interface IOption {
		void SetId();
		bool IsEnabled();
	}
}