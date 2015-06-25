using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timber_and_Stone;
using Timber_and_Stone.API.Event;
using Timber_and_Stone.Event;


namespace Plugin.HCR {
	public class MoreImmigrants : MonoBehaviour {

		private static MoreImmigrants instance = new MoreImmigrants();			
		public static MoreImmigrants getInstance() {
			return instance; 
		}

		public static int nextImmigrantDay = 0;

		///////////////////////////////////////////////////////////////////////////////////////////
		public void Start() {
			if (!Configuration.getInstance().isEnabledMoreImmigrants.getBool())
				return;
			
			Dbg.msg(Dbg.Grp.Startup,3,"More migrants started");		
			StartCoroutine(doImmigrants(10.0F));
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		
		IEnumerator doImmigrants(float waitTime) {
			int cDay, cHour;
			TimeManager tm = AManager<TimeManager>.getInstance();
			
			while (true) {
				yield return new WaitForSeconds(waitTime);
				try {		
					cDay = tm.day;
					cHour = tm.hour;
					
					if(
						(cDay >= nextImmigrantDay) && (cHour >= (UnityEngine.Random.Range(8,14)) && (cHour <= (UnityEngine.Random.Range(10,18))))
					) {
						UnitManager um = AManager<UnitManager>.getInstance();
						int settlers = um.LiveUnitCount();
						ResourceManager rm = AManager<ResourceManager>.getInstance();
						int food = rm.materials[4];

						Dbg.msg(Dbg.Grp.Units,1,"Checking immigration:"+food.ToString()+"food for "+settlers.ToString()+" settlers");
						//given food spoilage it's kinda hard to have over 3000 food so this will stop at about 30 settlers
						if ((food/settlers) >= (50+settlers*1.5)) {
							Dbg.msg(Dbg.Grp.Units,1,"Trying immigrant");
							um.Migrate(Vector3.zero);	//chance of 1/4, ugh
						}
					}
				} catch(Exception e) { 
					Dbg.dumpCorExc("doImmigrants",e);
				}
			}	
		}			

		///////////////////////////////////////////////////////////////////////////////////////////
		
		public static void processEvent(ref EventMigrant evt) {
			TimeManager tm = AManager<TimeManager>.getInstance();
			if (nextImmigrantDay <= tm.day) {
				nextImmigrantDay = tm.day+UnityEngine.Random.Range(1,3);
			} else {
				nextImmigrantDay = nextImmigrantDay+UnityEngine.Random.Range(1,3);
			}
			UI.print("Someone's coming over the hills. I think it's one of us..");

			Dbg.msg(Dbg.Grp.Units,3,"Immigrant event processed, new immigrant check set to day: "+nextImmigrantDay);
		}		
	}
}

