using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Timber_and_Stone;

namespace Plugin.HCR {
	public class MoreImmigrants : MonoBehaviour {

		private static MoreImmigrants instance = new MoreImmigrants();			
		public static MoreImmigrants getInstance() {
			return instance; 
		}

		int ticks = 0;

		///////////////////////////////////////////////////////////////////////////////////////////

		public void Start() {
			ticks = 0;
			Dbg.msg(Dbg.Grp.Startup,3,"More migrants started");		
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		
		public void Update() {
			try {
				ticks++;
				if ((Configuration.getInstance().isEnabledMoreImmigrants.getBool()) && (ticks >= 128)) {
					ticks = 0;
				}
			} catch(Exception e) { 
				Dbg.dumpExc(e);
			}
		}
		
		
	}
}

