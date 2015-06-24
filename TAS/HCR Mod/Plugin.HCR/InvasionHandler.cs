using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Timber_and_Stone;
using Timber_and_Stone.API;
using Timber_and_Stone.API.Event;
using Timber_and_Stone.Event;


namespace Plugin.HCR {
	public class InvasionHandler {

		public static void processEvent(ref EventInvasion evt) {
			TimeManager tm = AManager<TimeManager>.getInstance();
			Configuration conf = Configuration.getInstance();
			
			evt.result = Result.Default;
			int circleBreaker = 0;
			do {
				switch(evt.enemyType) {
					case 1:
						//goblins
						if (tm.day >= conf.noGoblinsTillDay.get()) {
							UI.print("Patrols found the remnants of a campfire. I wonder who's out there.");
							evt.result = Result.Allow;
						} else {
							if (conf.replaceMonsters.get() != 0) {
								Dbg.msg(Dbg.Grp.Invasion,3,"Replacing enemy type "+evt.enemyType.ToString()+" with " + conf.replaceGoblinsWith.get());								
								evt.enemyType = conf.replaceGoblinsWith.get();
							} else {
								evt.result = Result.Deny;
							}
						}
						break;
					case 2:
						//skeletons
						if (tm.day >= conf.noSkeletonsTillDay.get()) {
							UI.print("The forager says he heard a spooky sound in the bushes, like bones hitting on bones");
							evt.result = Result.Allow;
						} else {
							if (conf.replaceMonsters.get() != 0) {
								Dbg.msg(Dbg.Grp.Invasion,3,"Replacing enemy type "+evt.enemyType.ToString()+" with " + conf.replaceSkeletonsWith.get());								
								evt.enemyType = conf.replaceSkeletonsWith.get();
							} else {
								evt.result = Result.Deny;
							}
						}
						break;
					case 3:
						//spiders
						if (tm.day >= conf.noSpidersTillDay.get()) {
							UI.print("The woodchopper said the forest is full of spider webs");
							evt.result = Result.Allow;
						} else {
							evt.result = Result.Deny;
						}
						break;
					case 4:
						//wolves
						if (tm.day >= conf.noWolvesTillDay.get()) {
							UI.print("The herder says the sheep seem very agitated today");
							evt.result = Result.Allow;
						} else {
							if (conf.replaceMonsters.get() != 0) {
								Dbg.msg(Dbg.Grp.Invasion,3,"Replacing enemy type "+evt.enemyType.ToString()+" with " + conf.replaceWolvesWith.get());								
								evt.enemyType = conf.replaceWolvesWith.get();
							} else {
								evt.result = Result.Deny;
							}
						}
						break;
					case 5:
						//necromancer
						if (tm.day >= conf.noNecromancersTillDay.get()) {
							UI.print("There's a forestfire over the hills.");
							evt.result = Result.Allow;
						} else {
							if (conf.replaceMonsters.get() != 0) {
								Dbg.msg(Dbg.Grp.Invasion,3,"Replacing enemy type "+evt.enemyType.ToString()+" with " + conf.replaceNecromancersWith.get());								
								evt.enemyType = conf.replaceNecromancersWith.get();
							} else {
								evt.result = Result.Deny;
							}
						}
						break;
						
					default:
						UI.print("Something strange is coming, I can feel it... I'm afraid.");
						evt.result = Result.Allow;
						break;
				}
				if (circleBreaker++ >= 99) {
					Dbg.printErr("Couldn't replace monster type in invasion event. Replacement definition probably circular or invalid");
					evt.result = Result.Deny; 
				}
			} while (evt.result == Result.Default);
			if (evt.result == Result.Deny) {
				Dbg.msg(Dbg.Grp.Invasion,3,"Invasion of enemy type "+evt.enemyType.ToString()+" denied");								
				UI.print("It's been quiet lately. Too quiet...");
			} else {
				Dbg.msg(Dbg.Grp.Invasion,3,"Invasion of enemy type "+evt.enemyType.ToString()+" allowed");								
			}		
		}

	}
}

