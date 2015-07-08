using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timber_and_Stone;


namespace Plugin.HCR {
	
	public class Cheats : ExtMonoBehaviour {
		
		public static bool furCheatDone = false;
		
		///////////////////////////////////////////////////////////////////////////////////////////

		public override void Awake() {
		}
		
		public void Start() {
			if(!Configuration.getInstance().isEnabledCheats.getBool()) {
				return;
			}
			
			Dbg.trc(Dbg.Grp.Startup, 3);		
			StartCoroutine(doCheats(5.0F));
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		
		IEnumerator doCheats(float waitTime) {
			TimeManager tm = AManager<TimeManager>.getInstance();
			UnitManager um = AManager<UnitManager>.getInstance();
			ResourceManager rm = AManager<ResourceManager>.getInstance();
			int cDay, cHour;
			
			while(true) {
				yield return new WaitForSeconds(waitTime);
				
				try {		
					cDay = tm.day;
					cHour = tm.hour;

					//animal fur cheat, gives a chance for animla found on 1st day
					if((cDay == 1) && !furCheatDone && (UnityEngine.Random.Range(0, 200) == 0)) {
						Dbg.trc(Dbg.Grp.Else, 3, "furCheat done");		
						rm.AddResource(11, 5);
						UI.print("The woodchopper found some pieces of animal fur. Have any use for these?");
						furCheatDone = true;
					}
				
					//stuff up dead enemy inventories a little; 10 animal hide, 18 scrap metal, 47 leather, 55 coin	
					//TODO: this does dead *and* living, is there a deadunitlist? check for health?
					for(int i = 0; i < um.enemyUnits.Count; i++) {
						Enemy enemy = um.enemyUnits[i].GetComponent<Enemy>();
						if(enemy.unitName.StartsWith("Skele") && !enemy.inventory.Contains(18)) {
							enemy.inventory.Add(18, UnityEngine.Random.Range(0, 3));
						}
						if(enemy.unitName.StartsWith("Goblin") && !enemy.inventory.Contains(47)) {
							enemy.inventory.Add(47, UnityEngine.Random.Range(0, 3));
						}
					}
					Dbg.trc(Dbg.Grp.Else, 3, "enemy stuff done");		
					
				} catch(Exception e) { 
					Dbg.dumpCorExc("doCheats", e);
				}
			}	
		}		
	}
}
