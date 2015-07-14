using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timber_and_Stone;
using Timber_and_Stone.API.Event;
using Timber_and_Stone.Event;


namespace Plugin.HCR {
	public class Immigrants : Entity {

		public static int lastImmigrantDay = 0;
		public static int nextImmigrantDay = 0;
		
		///////////////////////////////////////////////////////////////////////////////////////////

		public override void Awake() {
			Dbg.trc(Dbg.Grp.Init, 3);
		}

		public void Start() {
			if(!Configuration.getInstance().isEnabledMoreImmigrants.getBool()) {
				return;
			}
			
			Dbg.trc(Dbg.Grp.Startup, 3);		
			StartCoroutine(doImmigrants(10.0F));
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		
		IEnumerator doImmigrants(float waitTime) {
			TimeManager tm = AManager<TimeManager>.getInstance();
			UnitManager um = AManager<UnitManager>.getInstance();
			ResourceManager rm = AManager<ResourceManager>.getInstance();
			int cDay, cHour;
			
			while(true) {
				yield return new WaitForSeconds(waitTime);
				try {		
					cDay = tm.day;
					cHour = tm.hour;			
					if(
						(cDay >= nextImmigrantDay) && (cHour >= (UnityEngine.Random.Range(8, 14)) && (cHour <= (UnityEngine.Random.Range(10, 18))))
					) {
						int settlers = um.LiveUnitCount();
						int food = rm.materials[4];

						Dbg.msg(Dbg.Grp.Units, 3, "Checking immigration: " + food.ToString() + " food for " + settlers.ToString() + " settlers");
						//never had much over 3000 food in my games so I guess this will normally stop at about 30 settlers
						//might stil get some more from in game logic
						if((food / settlers) >= (50 + settlers * 1.5)) {
							Dbg.msg(Dbg.Grp.Units, 3, "Trying immigrant");
							um.Migrate(Vector3.zero);	//chance of 1/4, ugh
						}
					}
				} catch(Exception e) { 
					Dbg.dumpCorExc("doImmigrants", e);
				}
				
			}	
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		
		public static void processEvent(ref EventMigrant evt) {
			TimeManager tm = AManager<TimeManager>.getInstance();
			if(nextImmigrantDay <= tm.day) {
				nextImmigrantDay = tm.day + UnityEngine.Random.Range(1, 3);
			} else {
				nextImmigrantDay = nextImmigrantDay + UnityEngine.Random.Range(1, 3);
			}
			lastImmigrantDay = tm.day;
			UI.print("Someone's coming over the hills. I think it's one of us..");

			Dbg.msg(Dbg.Grp.Units, 3, "Immigrant event processed, new immigrant check set to day: " + nextImmigrantDay);
		}
		
	}
}

