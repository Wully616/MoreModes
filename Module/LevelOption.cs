using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;

namespace GameModeLoader.Module {

	/// <summary>
	/// levelOption is temporarily added at run time, it contains a levelModule
	/// </summary>
	public class LevelOption : LevelData.Option {
		public LevelModule levelModule;
	}
}
