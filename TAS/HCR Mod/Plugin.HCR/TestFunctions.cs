using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Timber_and_Stone;

namespace Plugin.HCR {
	public class TestFunctions {

		public static KeyCommands.CmdFunc[] tf = {
			halp,
			weatherEvent,
			tryImmigrant,
			dumpTraitsDict,
			dumpTreeData,
			dumpTradeResources,
			dumpTrackedResources,
			dumpDeadEnemies,
			stuffDeadEnemies,
			toggleHookMouseEvents,
			unassignedTestFunc,
			unassignedTestFunc
		};


		//*****************************************************************************************		
		public static void incDebugLvl() {

			Configuration conf = Configuration.getInstance();
			if(conf.isEnabledDebugLevel.get() < 10) {
				conf.isEnabledDebugLevel.set(conf.isEnabledDebugLevel.get() + 1);
			}			
			Dbg.printMsg("Debug level increased, now set to: " + conf.isEnabledDebugLevel.get().ToString());
		}

		//*****************************************************************************************		
		public static void decDebugLvl() {
			
			Configuration conf = Configuration.getInstance();
			if(conf.isEnabledDebugLevel.get() > 0) {
				conf.isEnabledDebugLevel.set(conf.isEnabledDebugLevel.get() - 1);			
				Dbg.printMsg("Debug level decreased , now set to: " + conf.isEnabledDebugLevel.get().ToString());
			}
		}

		//*****************************************************************************************		
		public static void halp() {
			Dbg.printMsg("Forgot it *again*? you dense.. ");
			int i = 1;
			
			foreach (KeyCommands.CmdFunc cmd in tf) {
				UI.print("F"+i.ToString("D2")+": "+cmd.Method.Name);
				i++;
			}
		}					
		
		//*****************************************************************************************		
		public static void weatherEvent() {
			Dbg.printMsg("Test invoked: " + MethodBase.GetCurrentMethod().Name);
			
			TimeManager tm = AManager<TimeManager>.getInstance();
			Weather weather = SingletonEntity<HCRMod>.GetEntity<Weather>();
			Rain rain = Weather.GetEntity<Rain>();
			rain.timeToRemove = Time.time;
			rain.removeRain();
			weather.nextRainDay = tm.day;
			weather.nextRainHour = tm.hour;
		}
		
		//*****************************************************************************************		
		public static void tryImmigrant() {
			Dbg.printMsg("Test invoked: " + MethodBase.GetCurrentMethod().Name);

			TimeManager tm = AManager<TimeManager>.getInstance();
			Immigrants.nextImmigrantDay = tm.day;
		}
		
		//*****************************************************************************************		
		public static void dumpTraitsDict() {
			Dbg.printMsg("Test invoked: " + MethodBase.GetCurrentMethod().Name);

//			foreach (var kvp in ImproveUnitTraits.unitLevelUpEvents) {
//				UI.print(kvp.Key.ToString()+"="+kvp.Value.ToString());
//			}
		}
		
		//*****************************************************************************************		
		public static void dumpTreeData() {
			Dbg.printMsg("Test invoked: " + MethodBase.GetCurrentMethod().Name);

			TerrainObjectManager tm = AManager<TerrainObjectManager>.getInstance();
			
			foreach(TreeFlora treeObj in tm.treeObjects) {
				UI.print(treeObj.treeIndex.ToString("D3") + ":" + treeObj.transform.position.ToString() + ":" + treeObj.onFire.ToString() + ":" + treeObj.health.ToString());
			}
		}
		
		//*****************************************************************************************		
		public static void dumpTradeResources() {
			Dbg.printMsg("Test invoked: " + MethodBase.GetCurrentMethod().Name);
			
			ResourceManager rm = AManager<ResourceManager>.getInstance();
			foreach(Resource res in rm.sellList) {
				Dbg.printMsg(res.name.ToString());
			}
			Dbg.printMsg("Test invoked: show trade/buy resources");
			foreach(Resource res in rm.buyList) {
				Dbg.printMsg(res.name.ToString());
			}
		}
		
		//*****************************************************************************************		
		public static void dumpTrackedResources() {
			Dbg.printMsg("Test invoked: " + MethodBase.GetCurrentMethod().Name);
			
			
			GUIManager gm = AManager<GUIManager>.getInstance();
			ResourceManager rm = AManager<ResourceManager>.getInstance();
			foreach(int resId in gm.watchedResources) {
				Dbg.printMsg(resId.ToString("D3") + ":" + rm.resources[resId].name.ToString() + "= " + rm.materials[resId].ToString());
			}
		}
		
		//*****************************************************************************************		
		public static void dumpDeadEnemies() {
			Dbg.printMsg("Test invoked: " + MethodBase.GetCurrentMethod().Name);
						
			UnitManager um = AManager<UnitManager>.getInstance();
			ResourceManager rm = AManager<ResourceManager>.getInstance();
			
			for(int i = 0; i < um.enemyUnits.Count; i++) {
				Enemy enemy = um.enemyUnits[i].GetComponent<Enemy>();
				Dbg.printMsg(enemy.name.ToString() + ":" + enemy.unitName.ToString());
				foreach(var kvp in enemy.inventory) {
					//Dbg.printMsg(" "+rm.resources[kvp.Key].name+":"+kvp.Value.ToString()); 
					Dbg.printMsg(" " + kvp.Key.name + ":" + kvp.Value.ToString());
				}
			}
		}
		
		//*****************************************************************************************		
		//  10 animal hide, 18 scrap metal, 47 leather, 55 coin
		public static void stuffDeadEnemies() {
			Dbg.printMsg("Test invoked: " + MethodBase.GetCurrentMethod().Name);
			
			UnitManager um = AManager<UnitManager>.getInstance();
			ResourceManager rm = AManager<ResourceManager>.getInstance();
			
			for(int i = 0; i < um.enemyUnits.Count; i++) {
				Enemy enemy = um.enemyUnits[i].GetComponent<Enemy>();
				if(enemy.unitName.StartsWith("Skele") && !enemy.inventory.Contains(18)) {
					enemy.inventory.Add(18, UnityEngine.Random.Range(0, 3));
				}
				if(enemy.unitName.StartsWith("Goblin") && !enemy.inventory.Contains(47)) {
					enemy.inventory.Add(47, UnityEngine.Random.Range(0, 3));
				}
			}
		}

	
		//*****************************************************************************************		

		public static void toggleHookMouseEvents() {
			Dbg.printMsg("Test invoked: " + MethodBase.GetCurrentMethod().Name);
			KeyCommands.doHookMouseEvents = !KeyCommands.doHookMouseEvents;
		}
			
		public static void HookMouseEvents() {
			ChunkManager cm = AManager<ChunkManager>.getInstance();
			RaycastHit raycastHit;
			Vector3[] array;
			string str;

			if(Physics.Raycast(Camera.current.ScreenPointToRay(Input.mousePosition), out raycastHit)) {
				if(raycastHit.transform.tag == "PickPlane") {
					Vector3 normalized = (raycastHit.point - Camera.current.transform.position).normalized;
					array = cm.Pick(raycastHit.point, normalized, true);
				} else {
					array = cm.Pick(Camera.current.transform.position, Camera.current.ScreenPointToRay(Input.mousePosition).direction, true);
				}
				Vector3 pos = cm.GetWorldPosition(array[0], array[1]);
				Vector3i abs = new Vector3i((int)(array[0].x*cm.chunkSize.x+array[1].x),(int)array[0].y,(int)(array[0].z*cm.chunkSize.z+array[1].z));
				str =
					"World: "+pos.ToString()+"\n"+
						"Chunk: "+array[0].ToString()+"\n"+
						"ChkBlk: "+array[1].ToString()+"\n"+
						"AbsBlk: "+abs.ToString()+"\n"+
						"Hit: "+raycastHit.transform.tag;
				;
			} else {
				array = cm.Pick(Camera.current.transform.position);
				Vector3 pos = cm.GetWorldPosition(array[0], array[1]);
				Vector3i abs = new Vector3i((int)(array[0].x*cm.chunkSize.x+array[1].x),(int)array[0].y,(int)(array[0].z*cm.chunkSize.z+array[1].z));
				str =
					"World: "+pos.ToString()+"\n"+
						"Chunk: "+array[0].ToString()+"\n"+
						"ChkBlk: "+array[1].ToString()+"\n"+
						"AbsBlk: "+abs.ToString()+"\n"
				;
			}
			UI.guiText.transform.position = Camera.main.ScreenToViewportPoint(Input.mousePosition);
			UI.guiText.text = str; 
		}


		//*****************************************************************************************		
		public static void unassignedTestFunc() {
			Dbg.printMsg("Test invoked: " + MethodBase.GetCurrentMethod().Name);
		}

		//*****************************************************************************************		

//		public class Hack : MonoBehaviour {
//			
//			public bool isHackOnMap = false;
//			
//			private static Hack instance = new Hack();			
//			public static Hack getInstance() {
//				return instance; 
//			}
//			
//			public Hack() {
//			} 
//			
//			public void Start() {
//				Dbg.trc(Dbg.Grp.Startup,5,"Toogle road designation Hack started");
//			}
//			
//			public void Update() {
//				Dbg.msg(Dbg.Grp.Startup,5,".");
//				DesignManager dm = AManager<DesignManager>.getInstance();
//				dm.roadDesignation.renderer.enabled = false;
//				dm.roadTexture.color = Color.clear;
//				//dm.roadDesignation.transform.GetComponent<Material>().color = Color.clear;
//			}			
//		}
//		
//		public static void toggleDesignations() {
//			Dbg.printMsg("Test invoked: "+MethodBase.GetCurrentMethod().Name);
//
//			AManager<GUIManager>.getInstance().gameObject.AddComponent(typeof(Hack));
//	
//			DesignManager dm = AManager<DesignManager>.getInstance();
//			dm.roadDesignation.renderer.enabled = false;
//			//dm.roadDesignation.transform.GetComponent<Material>().color = Color.clear;
// 		}		


	}
}

