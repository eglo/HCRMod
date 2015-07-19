using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Timber_and_Stone;

namespace Plugin.HCR {
	
	public class HCRModWrapper : SingletonEntity<HCRModWrapper> {
			
		public override void Awake() {
			Dbg.trc(Dbg.Grp.Init, 5);
			
			HCRMod hcr = AddEntity<HCRMod>(this.transform);
		}
	}

	public class HCRMod : SingletonEntity<HCRMod> {


		public override void Awake() {
			Dbg.trc(Dbg.Grp.Init, 5);			
		}
		
		public void Start() {
			Dbg.trc(Dbg.Grp.Startup, 5);			
			
			StartCoroutine(initHCRMod(0.1F));

		}

		IEnumerator initHCRMod(float waitTime) {

			//wait for game load, some parts of the game are not quite init'ed at this time.
			GUIManager gm = AManager<GUIManager>.getInstance();
			int ticks = 0;
			while(!gm.inGame) {
				Dbg.msg(Dbg.Grp.Init, 3, "Wait for game init.." + ticks.ToString());
				yield return new WaitForSeconds(1.0f);
				ticks++;
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
				conf.isEnabledDebugLevel.set(5);
				conf.isEnabledDebugGroup.set((int)(
				Dbg.Grp.Init|Dbg.Grp.Startup|Dbg.Grp.Unity|Dbg.Grp.Time|Dbg.Grp.Terrain|Dbg.Grp.Weather|Dbg.Grp.Units|Dbg.Grp.Invasion
				//Dbg.Grp.Init | Dbg.Grp.Startup | Dbg.Grp.Sound | Dbg.Grp.Terrain | Dbg.Grp.Weather
));

				Dbg.trc(Dbg.Grp.Init, 5);				
				if(conf.isEnabledWeatherEffects.getBool()) {
					AddEntity<Weather>(this.transform);
					UI.print("Rainblobs visible effect" + conf.isEnabledShowRainBlocks.toEnabledString());
					
				}
				UI.print("Weather effects" + conf.isEnabledWeatherEffects.toEnabledString());

				if(conf.isEnabledImproveUnitTraits.getBool()) {
					AddEntity<UnitTraits>(this.transform);
				}			
				UI.print("Improve unit traits" + conf.isEnabledImproveUnitTraits.toEnabledString());

				if(conf.isEnabledMoreImmigrants.getBool()) {
					AddEntity<Immigrants>(this.transform);
				}			
				UI.print("More immigrants" + conf.isEnabledMoreImmigrants.toEnabledString());

				if(conf.isEnabledMoreMerchants.getBool()) {
					AddEntity<Merchants>(this.transform);
				}			
				UI.print("More merchants" + conf.isEnabledMoreMerchants.toEnabledString());

				if(conf.isEnabledCheats.getBool()) {
					AddEntity<Cheats>(this.transform);
				}			
				UI.print("Cheats" + conf.isEnabledCheats.toEnabledString());

				if(conf.isEnabledKeyboardCommands.getBool()) {
					AddEntity<KeyboardCommands>(this.transform);
				}			
				UI.print("Keyboard commands" + conf.isEnabledKeyboardCommands.toEnabledString());
				
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
				
				Dbg.msg(Dbg.Grp.Startup, 5, "Mod enabled");
			} catch(Exception e) {
				Dbg.dumpCorExc("initHCRMod", e);
			}			
		}
	}
}

