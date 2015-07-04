using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Timber_and_Stone;

namespace Plugin.HCR {

	public class HCRMod : MonoBehaviour {

		public static GameObject go = new GameObject();

		private static HCRMod instance = new HCRMod();			
		public static HCRMod getInstance() {
			return instance; 
		}

		public HCRMod() {
		}

		public void Start() {
			StartCoroutine(initHCRMod(0.1F));
		}

		public void OnLevelWasLoaded(int level) {
			Dbg.trc(Dbg.Grp.Init, 1, "Level loaded:" + level.ToString());
		}

		IEnumerator initHCRMod(float waitTime) {

			//seems OnEnable comes a little too early, apparently some parts of the game are not quite init'ed at this time.
			//(I'm looking at you, worldSize member in ChunkManager..)
			GUIManager gm = AManager<GUIManager>.getInstance();
			int ticks = 0;
			while(!gm.inGame) {
				Dbg.msg(Dbg.Grp.Init, 1, "Not in game:" + ticks.ToString());
				yield return new WaitForSeconds(3.0f);
			}

			try {
				Configuration conf = Configuration.getInstance();
				
				gm.AddTextLine("HCR - Here Comes The Rain - Mod Version " + conf.version + " Build " + conf.build);
				if(!conf.init()) {
					Dbg.printErr("Error in configuration init, mod is NOT enabled");
					yield break;
				}
	
//TODO_ delete this			
//overrides ini config...
				conf.isEnabledDebugLevel.set(3);
				conf.isEnabledDebugGroup.set((int)(
				//Dbg.Grp.Init|Dbg.Grp.Startup|Dbg.Grp.Unity|Dbg.Grp.Time|Dbg.Grp.Map|Dbg.Grp.Weather|Dbg.Grp.Units|Dbg.Grp.Invasion
					Dbg.Grp.Init | Dbg.Grp.Startup | Dbg.Grp.Terrain | Dbg.Grp.Weather | Dbg.Grp.Rain | Dbg.Grp.Sound
));

				Dbg.trc(Dbg.Grp.Init, 1);				
				UI.print("Rain effects" + conf.isEnabledWeatherEffects.toEnabledString());
				if(conf.isEnabledWeatherEffects.getBool()) {
					go.AddComponent(typeof(Weather));
					UI.print("Rainblobs visible effect" + conf.isEnabledShowRainBlocks.toEnabledString());
					
				}
				UI.print("Improve unit traits" + conf.isEnabledImproveUnitTraits.toEnabledString());
				if(conf.isEnabledImproveUnitTraits.getBool()) {
					go.AddComponent(typeof(UnitTraits));
				}			
				UI.print("More immigrants" + conf.isEnabledMoreImmigrants.toEnabledString());
				if(conf.isEnabledMoreImmigrants.getBool()) {
					go.AddComponent(typeof(Immigrants));
				}			
				UI.print("More merchants" + conf.isEnabledMoreMerchants.toEnabledString());
				if(conf.isEnabledMoreMerchants.getBool()) {
					go.AddComponent(typeof(Merchants));
				}			
				UI.print("Cheats" + conf.isEnabledCheats.toEnabledString());
				if(conf.isEnabledCheats.getBool()) {
					//go.AddComponent(typeof(Immigrants));
				}			
				UI.print("Keyboard commands" + conf.isEnabledKeyboardCommands.toEnabledString());
				if(conf.isEnabledKeyboardCommands.getBool()) {
					go.AddComponent(typeof(KeyboardCommands));
				}			
				UI.print("Invasion configuration" + conf.isEnabledInvasionConfig.toEnabledString());
				
				if(conf.trackResourcesIdxFirst.getBool() && conf.trackResourcesIdxLast.getBool()) {
					ResourceManager rm = AManager<ResourceManager>.getInstance();
					//this will only happen for a game started anew, not for a saved game
					if(gm.watchedResources.Count == 0) {
						for(int i = conf.trackResourcesIdxFirst.get(); i <= conf.trackResourcesIdxLast.get(); i++) {
							if((i >= 0) && (i < rm.resources.Length)) {
								gm.watchedResources.Add(i);
							}
						}
					}						
				}
				
				if(conf.isEnabledDebugLevel.getBool()) {
					UI.print("Debug enabled level:" + conf.isEnabledDebugLevel.get().ToString());
					Dbg.Grp grp = (Dbg.Grp)conf.isEnabledDebugGroup.get();
					UI.print("Debug enabled groups:" + grp.ToString());
				}
				
				Dbg.msg(Dbg.Grp.Startup, 3, "Mod enabled");
			} catch(Exception e) {
				Dbg.dumpCorExc("initHCRMod", e);
			}			
		}

	}
}

