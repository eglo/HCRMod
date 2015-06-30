using System;
using System.Collections;
using UnityEngine;
using Timber_and_Stone;

namespace Plugin.HCR {

	public class HCRMod : MonoBehaviour {

		public static GameObject go  = new GameObject();

		private static HCRMod instance = new HCRMod();			
		public static HCRMod getInstance() {
			return instance; 
		}

		public HCRMod() {
		}

		public void Start() {
			StartCoroutine(initHCRMod(0.1F));
		}

		IEnumerator initHCRMod(float waitTime) {
			
			//seems OnEnable comes a little too early, apparently some parts of the game are not quite init'ed at this time.
			//(I'm looking at you, worldSize member in ChunkManager..)
			GUIManager gm = AManager<GUIManager>.getInstance();
			while(!gm.inGame)
				yield return new WaitForSeconds(0.1f);

			try {
				Configuration conf = Configuration.getInstance();
				
				gm.AddTextLine("HCR - Here Comes The Rain - Mod Version "+conf.version+" Build "+conf.build);
				if (!conf.init()) {
					Dbg.printErr("Error in configuration init, mod is NOT enabled");
					yield break;
				}
				
				//overrides ini config...
				conf.isEnabledDebugLevel.set(3);
				conf.isEnabledDebugGroup.set((int)(
					//Dbg.Grp.Init|Dbg.Grp.Startup|Dbg.Grp.Unity|Dbg.Grp.Time|Dbg.Grp.Map|Dbg.Grp.Weather|Dbg.Grp.Units|Dbg.Grp.Invasion
					Dbg.Grp.Startup|Dbg.Grp.Map|Dbg.Grp.Weather|Dbg.Grp.Rain|Dbg.Grp.Units
				));

				Dbg.trc(Dbg.Grp.Init,1);				
				Dbg.printMsg("Rain effects"+conf.isEnabledWeatherEffects.toEnabledString());
				if (conf.isEnabledWeatherEffects.getBool()) {
					go.AddComponent(typeof(Weather));
					Dbg.printMsg("Rainblobs visible effect"+conf.isEnabledShowRainBlocks.toEnabledString());
					
				}
				Dbg.printMsg("Improve unit traits"+conf.isEnabledImproveUnitTraits.toEnabledString());
				if(conf.isEnabledImproveUnitTraits.getBool()) {
					go.AddComponent(typeof(UnitTraits));
				}			
				Dbg.printMsg("More immigrants"+conf.isEnabledMoreImmigrants.toEnabledString());
				if(conf.isEnabledMoreImmigrants.getBool()) {
					go.AddComponent(typeof(Immigrants));
				}			
				Dbg.printMsg("More merchants"+conf.isEnabledMoreMerchants.toEnabledString());
				if(conf.isEnabledMoreMerchants.getBool()) {
					go.AddComponent(typeof(Merchants));
				}			
				Dbg.printMsg("Cheats"+conf.isEnabledCheats.toEnabledString());
				if(conf.isEnabledCheats.getBool()) {
					//go.AddComponent(typeof(Immigrants));
				}			
				Dbg.printMsg("Keyboard commands"+conf.isEnabledKeyboardCommands.toEnabledString());
				if(conf.isEnabledKeyboardCommands.getBool()) {
					go.AddComponent(typeof(KeyboardCommands));
				}			
				Dbg.printMsg("Invasion configuration"+conf.isEnabledInvasionConfig.toEnabledString());
				
				if(conf.trackResourcesIdxFirst.getBool() && conf.trackResourcesIdxLast.getBool()) {
					ResourceManager rm = AManager<ResourceManager>.getInstance();
					//this will only happen for a game started anew, not for a saved game
					if(gm.watchedResources.Count == 0) {
						for (int i = conf.trackResourcesIdxFirst.get(); i <= conf.trackResourcesIdxLast.get(); i++) {
							if ((i >= 0) && (i < rm.resources.Length)) {
								gm.watchedResources.Add(i);
							}
						}
					}						
				}
				
				if (conf.isEnabledDebugLevel.getBool()) {
					Dbg.printMsg("Debug enabled level:"+conf.isEnabledDebugLevel.get().ToString());
					Dbg.Grp grp = (Dbg.Grp) conf.isEnabledDebugGroup.get();
					Dbg.printMsg("Debug enabled groups:"+grp.ToString());
				}
				
				Dbg.msg(Dbg.Grp.Startup,3,"Mod enabled");
			}  catch(Exception e) {
				Dbg.dumpCorExc("initHCRMod",e);
			}			
		}

	}
}

