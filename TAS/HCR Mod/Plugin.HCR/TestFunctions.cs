using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Timber_and_Stone;

namespace Plugin.HCR {
	public class TestFunctions {

		public static KeyboardCommands.CmdFunc[] tf = {
			weatherEvent,
			tryImmigrant,
			dumpTraitsDict,
			dumpTreeData,
			dumpTradeResources,
			dumpTrackedResources,
			unassignedTestFunc,
			unassignedTestFunc,
			unassignedTestFunc,
			unassignedTestFunc,
			unassignedTestFunc,
			unassignedTestFunc			
		};

		///////////////////////////////////////////////////////////////////////////////////////////		
		public static void weatherEvent() {
			Dbg.printMsg("Test invoked: "+MethodBase.GetCurrentMethod().Name);
			
			TimeManager tm = AManager<TimeManager>.getInstance();
			Weather.nextRainDay = tm.day;
			Weather.nextRainHour = tm.hour;
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////		
		public static void tryImmigrant() {
			Dbg.printMsg("Test invoked: "+MethodBase.GetCurrentMethod().Name);

			TimeManager tm = AManager<TimeManager>.getInstance();
			MoreImmigrants.nextImmigrantDay = tm.day;
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////		
		public static void dumpTraitsDict() {
			Dbg.printMsg("Test invoked: "+MethodBase.GetCurrentMethod().Name);

			Dbg.printMsg("Test invoked: dump traits dict");
			foreach (var kvp in ImproveUnitTraits.unitLevelUpEvents) {
				UI.print(kvp.Key.ToString()+"="+kvp.Value.ToString());
			}
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////		
		public static void dumpTreeData() {
			Dbg.printMsg("Test invoked: "+MethodBase.GetCurrentMethod().Name);
			
			foreach (var kvp in ImproveUnitTraits.unitLevelUpEvents) {
				UI.print(kvp.Key.ToString()+"="+kvp.Value.ToString());
			}
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////		
		public static void dumpTradeResources() {
			Dbg.printMsg("Test invoked: "+MethodBase.GetCurrentMethod().Name);
			
			ResourceManager rm = AManager<ResourceManager>.getInstance();
			foreach (Resource res in rm.sellList) {
				Dbg.printMsg(res.name.ToString());
			}
			Dbg.printMsg("Test invoked: show trade/buy resources");
			foreach (Resource res in rm.buyList) {
				Dbg.printMsg(res.name.ToString());
			}
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////		
		public static void dumpTrackedResources() {
			Dbg.printMsg("Test invoked: "+MethodBase.GetCurrentMethod().Name);
			
			
			GUIManager gm = AManager<GUIManager>.getInstance();
			ResourceManager rm = AManager<ResourceManager>.getInstance();
			foreach (int resId in gm.watchedResources) {
				Dbg.printMsg(resId.ToString("D3")+":"+rm.resources[resId].name.ToString());
			}
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////		
		public static void dumpDeadEnemies() {
			Dbg.printMsg("Test invoked: "+MethodBase.GetCurrentMethod().Name);
						
			UnitManager um = AManager<UnitManager>.getInstance();
			ResourceManager rm = AManager<ResourceManager>.getInstance();
			
			for (int i = 0; i < um.enemyUnits.Count; i++) {
				Enemy enemy = um.enemyUnits[i].GetComponent<Enemy>();
				Dbg.printMsg(enemy.name.ToString()+":"+enemy.unitName.ToString());
				foreach(var kvp in enemy.inventory) {
					//Dbg.printMsg(" "+rm.resources[kvp.Key].name+":"+kvp.Value.ToString()); 
					Dbg.printMsg(" "+kvp.Key.name+":"+kvp.Value.ToString());
				}
			}
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////		
		public static void unassignedTestFunc() {
			Dbg.printMsg("Test invoked: "+MethodBase.GetCurrentMethod().Name);
			
		}		
	}
}

