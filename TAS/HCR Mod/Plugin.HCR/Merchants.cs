using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timber_and_Stone;
using Timber_and_Stone.API.Event;
using Timber_and_Stone.Event;


namespace Plugin.HCR {

	public class Merchants : ExtMonoBehaviour {

		public static int lastMerchantDay = 0;
		public static int nextMerchantDay = 0;
		
		///////////////////////////////////////////////////////////////////////////////////////////

		public override void Awake() {
			Setup();
		}

		public void Start() {
			if(!Configuration.getInstance().isEnabledMoreImmigrants.getBool()) {
				return;
			}
			
			Dbg.trc(Dbg.Grp.Startup, 3);		
			StartCoroutine(doMerchants(10.0F));
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		
		IEnumerator doMerchants(float waitTime) {
			TimeManager tm = AManager<TimeManager>.getInstance();
			UnitManager um = AManager<UnitManager>.getInstance();
			ResourceManager rm = AManager<ResourceManager>.getInstance();
			int cDay, cHour;
			
			while(true) {
				yield return new WaitForSeconds(waitTime);
				
				try {		
					cDay = tm.day;
					cHour = tm.hour;

					if(um.MerchantOnMap()) {
						//called here every 10secs atm, but hardly possible to miss .. or is it?
						lastMerchantDay = cDay;
						nextMerchantDay = cDay + 1;
					}

					if(
						(cDay >= nextMerchantDay) && (cHour >= (UnityEngine.Random.Range(8, 14)) && (cHour <= (UnityEngine.Random.Range(10, 18))))
					) {
						//check amount and value of material marked for sale
						//the more value is for trade the higher the chance for a merchant spawn
						//the chance is pretty good though since this is called every 10secs anyway hah
						//TODO: something better..
						float sellValue = 0.0f;
						foreach(Resource res in rm.sellList) {
							sellValue += rm.materials[res.index] * res.value;
							Dbg.trc(Dbg.Grp.Units, 2, res.name + ":" + (rm.materials[res.index] * res.value).ToString());
						}
						
						//seeds valued at 10.0f..
						Dbg.msg(Dbg.Grp.Units, 3, "Checking merchant: sellValue=" + sellValue.ToString());
						if((sellValue) >= UnityEngine.Random.Range(300.0f, 3000.0f)) {
							Dbg.msg(Dbg.Grp.Units, 3, "Trying merchant");
							um.SpawnMerchant(Vector3.zero);
							
						}
					}					
				} catch(Exception e) { 
					Dbg.dumpCorExc("doMerchants", e);
				}
			}	
		}		
	}
}

