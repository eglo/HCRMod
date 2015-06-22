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
		}

		public override void OnEnable() {
			EventManager.getInstance().Register(this);

			Configuration conf = Configuration.getInstance();
			conf.init();
			
			Display.printMsg("- Here Comes The Rain - Mod Version "+conf.version);
			Display.printMsg("Rain effects"+conf.isEnabledWeatherEffects.toEnabledString());
			if (conf.isEnabledWeatherEffects.getBool()) {
				AManager<ChunkManager>.getInstance().gameObject.AddComponent(typeof(Weather));
				//AManager<TerrainObjectManager>.getInstance().gameObject.AddComponent(typeof(Weather));
				Display.printMsg("Rainblobs visible effect"+conf.isEnabledShowRainBlocks.toEnabledString());
				AManager<GUIManager>.getInstance().gameObject.AddComponent(typeof(Rain));
				AManager<ChunkManager>.getInstance().gameObject.AddComponent(typeof(Rain));
				AManager<TerrainObjectManager>.getInstance().gameObject.AddComponent(typeof(Rain));
				
			}
			Display.printMsg("Invasion configuration"+conf.isEnabledInvasionConfig.toEnabledString());
			Display.printMsg("Improve unit traits"+conf.isEnabledImproveUnitTraits.toEnabledString());
			if(conf.isEnabledImproveUnitTraits.getBool()) {
				AManager<UnitManager>.getInstance().gameObject.AddComponent(typeof(ImproveUnitTraits));
			}			
			Display.printMsg("Keyboard commands"+conf.isEnabledKeyboardCommands.toEnabledString());
			if(conf.isEnabledKeyboardCommands.getBool()) {
				AManager<GUIManager>.getInstance().gameObject.AddComponent(typeof(KeyboardCommands));
			}			
		}

		public override void OnDisable() {
		}

		[Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
		public void onMigrant(EventMigrant evt) {
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
			TimeManager tm = AManager<TimeManager>.getInstance();
			GUIManager gm = AManager<GUIManager>.getInstance();
			Configuration conf = Configuration.getInstance();

			evt.result = Result.Default;
			int circleBreaker = 0;
			do {
				switch(evt.enemyType) {
					case 1:
						//goblins
						if (tm.day >= conf.noGoblinsTillDay.get()) {
							gm.AddTextLine("Patrols found the remnants of a campfire. I wonder who's out there.");
							evt.result = Result.Allow;
						} else {
							if (conf.replaceMonsters.get() != 0) {
								evt.enemyType = conf.replaceGoblinsWith.get();
							} else {
								evt.result = Result.Deny;
							}
						}
						break;
					case 2:
						//skeletons
						if (tm.day >= conf.noSkeletonsTillDay.get()) {
							gm.AddTextLine("The forager says he heard a spooky sound in the bushes, like bones hitting on bones");
							evt.result = Result.Allow;
						} else {
							if (conf.replaceMonsters.get() != 0) {
								evt.enemyType = conf.replaceSkeletonsWith.get();
							} else {
								evt.result = Result.Deny;
							}
						}
						break;
					case 3:
						//spiders
						if (tm.day >= conf.noSpidersTillDay.get()) {
							gm.AddTextLine("The woodchopper said the forest is full of spider webs");
							evt.result = Result.Allow;
						} else {
							evt.result = Result.Deny;
						}
						break;
					case 4:
						//wolves
						if (tm.day >= conf.noWolvesTillDay.get()) {
							gm.AddTextLine("The herder says the sheep seem very agitated today");
							evt.result = Result.Allow;
						} else {
							if (conf.replaceMonsters.get() != 0) {
								evt.enemyType = conf.replaceWolvesWith.get();
							} else {
								evt.result = Result.Deny;
							}
						}
						break;
					case 5:
						//necromancer
						if (tm.day >= conf.noNecromancersTillDay.get()) {
							gm.AddTextLine("There's a forestfire over the hills.");
							evt.result = Result.Allow;
						} else {
							if (conf.replaceMonsters.get() != 0) {
								evt.enemyType = conf.replaceNecromancersWith.get();
							} else {
								evt.result = Result.Deny;
							}
						}
						break;

					default:
						gm.AddTextLine("Something strange is coming, I can feel it... I'm afraid.");
						evt.result = Result.Allow;
						break;
				}
				if (circleBreaker++ >= 99) {
					Display.printErr("Couldn't replace monster type in invasion event. Replacement definition probably circular or invalid");
					evt.result = Result.Deny; 
				}
			} while (evt.result == Result.Default);
			if (evt.result == Result.Deny) {
				gm.AddTextLine("It's been quiet lately. Too quiet...");
			}
		}


		[Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
		public void onInvasionMonitor(EventInvasion evt) {
			Result arg_0F_0 = evt.result;
		}
	}
}




//this.CalculateExperience();
//AManager<GUIManager>.getInstance().AddTextLine(string.Concat(new object[]
//                                                             {
//	"Level Up! ",
//	this.unit.unitName,
//	" is now a Lv. ",
//	this.level,
//	" ",
//	this.getProfessionName(),
//	"."
//}), this.unit.transform, false);



//AManager<UnitManager>.getInstance().Migrate(Vector3.zero);



