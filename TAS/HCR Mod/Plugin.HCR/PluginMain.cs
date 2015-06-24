using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Timber_and_Stone;
using Timber_and_Stone.API;
using Timber_and_Stone.API.Event;
using Timber_and_Stone.Event;
using Timber_and_Stone.Blocks;

namespace Plugin.HCR {
	public class PluginMain : CSharpPlugin, IEventListener {


		private static PluginMain instance;
		public static PluginMain getInstance() {
			return instance;
		}

		public override void OnLoad() {

			instance = this;
			EventManager.getInstance().Register(this);
			
		}

		public override void OnEnable() {
			//this comes to early, some parts of the game are not quite init'd at this time..
			try {
				Configuration conf = Configuration.getInstance();
				conf.init();
				
				conf.isEnabledDebugGroup.set((int)(
					//Dbg.Grp.Init|Dbg.Grp.Startup|Dbg.Grp.Unity|Dbg.Grp.Time|Dbg.Grp.Map|Dbg.Grp.Weather|Dbg.Grp.Units|Dbg.Grp.Invasion
					Dbg.Grp.Init|Dbg.Grp.Startup
				));
				conf.isEnabledDebugLevel.set(3);
				Dbg.trc(Dbg.Grp.Init,3);

				Dbg.msg(Dbg.Grp.Startup,3,"Mod enabled");
				Dbg.printMsg("- Here Comes The Rain - Mod Version "+conf.version);
				Dbg.printMsg("Rain effects"+conf.isEnabledWeatherEffects.toEnabledString());
				if (conf.isEnabledWeatherEffects.getBool()) {
					AManager<ChunkManager>.getInstance().gameObject.AddComponent(typeof(Weather));
					//AManager<TerrainObjectManager>.getInstance().gameObject.AddComponent(typeof(Weather));
					Dbg.printMsg("Rainblobs visible effect"+conf.isEnabledShowRainBlocks.toEnabledString());
					//AManager<ChunkManager>.getInstance().gameObject.AddComponent(typeof(Rain));
					AManager<TerrainObjectManager>.getInstance().gameObject.AddComponent(typeof(Rain));
					
				}
				Dbg.printMsg("Improve unit traits"+conf.isEnabledImproveUnitTraits.toEnabledString());
				if(conf.isEnabledImproveUnitTraits.getBool()) {
					AManager<UnitManager>.getInstance().gameObject.AddComponent(typeof(ImproveUnitTraits));
				}			
				Dbg.printMsg("More immigrants"+conf.isEnabledMoreImmigrants.toEnabledString());
				if(conf.isEnabledMoreImmigrants.getBool()) {
					AManager<UnitManager>.getInstance().gameObject.AddComponent(typeof(MoreImmigrants));
				}			
				Dbg.printMsg("Keyboard commands"+conf.isEnabledKeyboardCommands.toEnabledString());
				if(conf.isEnabledKeyboardCommands.getBool()) {
					AManager<GUIManager>.getInstance().gameObject.AddComponent(typeof(KeyboardCommands));
				}			
				Dbg.printMsg("Invasion configuration"+conf.isEnabledInvasionConfig.toEnabledString());
			}  catch(Exception e) {
				//does not work at this time, text windows isnt open yet.. 
				Dbg.dumpExc(e);
			}
		}

		public override void OnDisable() {
		}

		[Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
		public void onMigrant(EventMigrant evt) {
			Configuration conf = Configuration.getInstance();
			if(conf.isEnabledMoreImmigrants.getBool()) {
				MoreImmigrants.processEvent(ref evt);
			}			
		}

		[Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
		public void onMigrantAccept(EventMigrantAccept evt) {
		}

		[Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
		public void onEventCraft(EventCraft evt) {
		}

		[Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
		public void onMine(EventMine evt) {
		}

		[Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
		public void onEventGrow(EventBlockGrow evt) {
		}

		[Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
		public void onEventBlockSet(EventBlockSet evt) {
		}

		[Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
		public void onEventBuildStructure(EventBuildStructure evt) {
		}

		[Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
		public void onInvasionNormal(EventInvasion evt) {
	
			Configuration conf = Configuration.getInstance();
			if(!conf.isEnabledInvasionConfig.getBool()) {
				evt.result = Result.Allow;
				return;
			} else {
				InvasionHandler.processEvent(ref evt);
			}
		}


		[Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
		public void onInvasionMonitor(EventInvasion evt) {
			Result arg_0F_0 = evt.result;
		}
	}
}






