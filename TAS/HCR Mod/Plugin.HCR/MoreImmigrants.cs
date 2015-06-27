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

		public static int lastImmigrantDay = 0;
		public static int nextImmigrantDay = 0;
		public static int lastMerchantDay = 0;
		public static int nextMerchantDay = 0;
		
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
						//never had much over 3000 food in my games so I guess this will stop at about 30 settlers
						//might stil get some from in game logic
						if ((food/settlers) >= (50+settlers*1.5)) {
							Dbg.msg(Dbg.Grp.Units,1,"Trying immigrant");
							um.Migrate(Vector3.zero);	//chance of 1/4, ugh
						}
					}
				} catch(Exception e) { 
					Dbg.dumpCorExc("doImmigrants",e);
				}
				
				try {		
					cDay = tm.day;
					cHour = tm.hour;
					if(
						(cDay >= nextMerchantDay) && (cHour >= (UnityEngine.Random.Range(8,14)) && (cHour <= (UnityEngine.Random.Range(10,18))))
					) {
						//check amount and value of material marked for sale
						//the more you offer the higher the chance for a merchant coming
						//it's obviously highly exploitable as you can always deny the trade, but whatever.. ;)
						UnitManager um = AManager<UnitManager>.getInstance();
						ResourceManager rm = AManager<ResourceManager>.getInstance();
						float sellValue = 0.0f;
						foreach (Resource res in rm.sellList) {
							sellValue += rm.materials[res.index]* res.value;
							Dbg.printMsg(res.name.ToString());
						}
						
						Dbg.msg(Dbg.Grp.Units,1,"Checking merchant: "+sellValue.ToString());
						if ((sellValue) >= UnityEngine.Random.Range(50.0f,300.0f)) {
							Dbg.msg(Dbg.Grp.Units,1,"Trying immigrant");
							um.SpawnMerchant(Vector3.zero);
						}
					}
					
				} catch(Exception e) { 
					Dbg.dumpCorExc("doMerchants",e);
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
			lastImmigrantDay = tm.day;
			UI.print("Someone's coming over the hills. I think it's one of us..");

			Dbg.msg(Dbg.Grp.Units,3,"Immigrant event processed, new immigrant check set to day: "+nextImmigrantDay);
		}		
	}
}

