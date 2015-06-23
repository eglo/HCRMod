
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
			{KeyCode.T,test}
		};

		///////////////////////////////////////////////////////////////////////////////////////////
		
		public static void printHelp() {
			GUIManager gm = AManager<GUIManager>.getInstance();
			string str = "All commands use a combination of ";
			foreach (KeyCode key in cmdCombo) {
				str += (key.ToString()+"+");
			}
			str += "<key> for activation.";
			gm.AddTextLine(str);
			gm.AddTextLine("Command+H: help");
			gm.AddTextLine("Command+X: kill all enemies");
			gm.AddTextLine("Command+R: rain over full map");
			gm.AddTextLine("Command+F1: Game speed normal");
			gm.AddTextLine("Command+F2: Game speed 2x");
			gm.AddTextLine("Command+F3: Game speed 3x");
			gm.AddTextLine("Command+F4: Game speed 4x");
			gm.AddTextLine("Command+F5: Game speed 5x");
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
		public static void test() {
			Dbg.printMsg("Test invoked");	
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

