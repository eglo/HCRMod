
using System;
using System.Collections.Generic;
using UnityEngine;
using Timber_and_Stone;

namespace Plugin.HCR {
	public class KeyboardCommands : MonoBehaviour {

		private static KeyboardCommands instance = new KeyboardCommands();			
		public static KeyboardCommands getInstance() {
			return instance; 
		}
		
		private static KeyCode[] cmdCombo = {KeyCode.LeftShift,KeyCode.LeftControl,KeyCode.LeftAlt};

		public delegate void CmdFunc();
		private Dictionary<KeyCode,CmdFunc> cmdDict = new Dictionary<KeyCode,CmdFunc >{ 
			{KeyCode.H,printHelp},
			{KeyCode.X,killAllEnemies},
			{KeyCode.R,letItRain},
			{KeyCode.F1,setGameSpeed1},
			{KeyCode.F2,setGameSpeed2},
			{KeyCode.F3,setGameSpeed3},
			{KeyCode.F4,setGameSpeed4},
			{KeyCode.F5,setGameSpeed5},
			{KeyCode.T,test1},
			{KeyCode.Z,test2}
		};

		///////////////////////////////////////////////////////////////////////////////////////////
		
		public static void printHelp() {
			string str = "All commands use a combination of ";
			foreach (KeyCode key in cmdCombo) {
				str += (key.ToString()+"+");
			}
			str += "<key> for activation.";
			UI.print(str);
			UI.print("Command+H: help");
			UI.print("Command+X: kill all enemies");
			UI.print("Command+R: rain over full map");
			UI.print("Command+F1: Game speed normal");
			UI.print("Command+F2: Game speed 2x");
			UI.print("Command+F3: Game speed 3x");
			UI.print("Command+F4: Game speed 4x");
			UI.print("Command+F5: Game speed 5x");
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////

		public static void killAllEnemies() {
			AManager<UnitManager>.getInstance().killAllEnemies();
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		public static void letItRain() {
			Weather weather = Weather.getInstance();
			weather.rainStorm(0,0,weather.worldSize3i.x,weather.worldSize3i.z);
		}

		///////////////////////////////////////////////////////////////////////////////////////////		
		public static void setGameSpeed1() {
			AManager<TimeManager>.getInstance().play(1f);
			Dbg.printMsg("Game speed 1x");	
		}

		///////////////////////////////////////////////////////////////////////////////////////////		
		public static void setGameSpeed2() {
			AManager<TimeManager>.getInstance().play(2f);
			Dbg.printMsg("Game speed 2x");	
		}

		///////////////////////////////////////////////////////////////////////////////////////////		
		public static void setGameSpeed3() {
			AManager<TimeManager>.getInstance().play(3f);
			Dbg.printMsg("Game speed 3x");	
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		public static void setGameSpeed4() {
			AManager<TimeManager>.getInstance().play(4f);
			Dbg.printMsg("Game speed 4x");	
		}

		///////////////////////////////////////////////////////////////////////////////////////////		
		public static void setGameSpeed5() {
			AManager<TimeManager>.getInstance().play(5f);
			Dbg.printMsg("Game speed 5x");	
		}

		///////////////////////////////////////////////////////////////////////////////////////////		
		public static void test1() {
			Dbg.printMsg("Test invoked: do weather event..");

			TimeManager tm = AManager<TimeManager>.getInstance();
			Weather.nextRainDay = tm.day;
			Weather.nextRainHour = tm.hour;
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////		
		public static void test2() {
			Dbg.printMsg("Test invoked: try immigrant..");
			TimeManager tm = AManager<TimeManager>.getInstance();
			MoreImmigrants.nextImmigrantDay = tm.day;
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		
		public void Start() {
			Dbg.msg(Dbg.Grp.Startup,3,"Keyboard command handler started");
			
			string str;
			str = "Keyboard commands active. Press ";
			foreach (KeyCode key in cmdCombo) {
				str += (key.ToString()+"+");
			}
			str += "H for help.";
			Dbg.printMsg(str);
		}

		public void Update() {
		}
		
		public void OnGUI() {
			Event evt = Event.current;
			if (evt.isKey && checkPressed(cmdCombo)) {
				try {
					if(cmdDict.ContainsKey(evt.keyCode)) {
						cmdDict[evt.keyCode]();
					}
				} catch(Exception e) { 
					Dbg.dumpExc(e);
				}
			}
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////

		private bool checkPressed(KeyCode[] combo) {
			foreach(KeyCode key in combo) {
				if(!UnityEngine.Input.GetKey(key))
					return false;
			}
			return true;
		}

		private bool checkReleased(KeyCode[] combo) {
			foreach(KeyCode key in combo) {
				if(UnityEngine.Input.GetKey(key))
					return false;
			}
			return true;
		}
	}

	///////////////////////////////////////////////////////////////////////////////////////////

		
}

