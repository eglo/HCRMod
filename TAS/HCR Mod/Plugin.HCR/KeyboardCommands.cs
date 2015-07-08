using System;
using System.Collections.Generic;
using UnityEngine;
using Timber_and_Stone;

namespace Plugin.HCR {
	public class KeyboardCommands : ExtMonoBehaviour {

		
		private static KeyCode[] cmdCombo = {KeyCode.LeftShift,KeyCode.LeftControl,KeyCode.LeftAlt};

		public delegate void CmdFunc();
		private Dictionary<KeyCode,CmdFunc> cmdDict = new Dictionary<KeyCode,CmdFunc >{ 
			{KeyCode.H,printHelp},
			{KeyCode.X,killAllEnemies},
			{KeyCode.R,letItRain},
			{KeyCode.Alpha1,setGameSpeed1},
			{KeyCode.Alpha2,setGameSpeed2},
			{KeyCode.Alpha3,setGameSpeed3},
			{KeyCode.Alpha4,setGameSpeed4},
			{KeyCode.Alpha5,setGameSpeed5},
			{KeyCode.KeypadPlus,TestFunctions.incDebugLvl},
			{KeyCode.KeypadMinus,TestFunctions.decDebugLvl},			
			{KeyCode.F1,TestFunctions.tf[0]},
			{KeyCode.F2,TestFunctions.tf[1]},
			{KeyCode.F3,TestFunctions.tf[2]},
			{KeyCode.F4,TestFunctions.tf[3]},
			{KeyCode.F5,TestFunctions.tf[4]},
			{KeyCode.F6,TestFunctions.tf[5]},
			{KeyCode.F7,TestFunctions.tf[6]},
			{KeyCode.F8,TestFunctions.tf[7]},
			{KeyCode.F9,TestFunctions.tf[8]},
			{KeyCode.F10,TestFunctions.tf[9]}
		};

		///////////////////////////////////////////////////////////////////////////////////////////
		
		public static void printHelp() {
			string str = "All commands use a combination of ";
			foreach(KeyCode key in cmdCombo) {
				str += (key.ToString() + "+");
			}
			str += "<key> for activation.";
			UI.print(str);
			UI.print("Command+H: help");
			UI.print("Command+X: kill all enemies");
			UI.print("Command+R: start rain");
			UI.print("Command+1: Game speed normal");
			UI.print("Command+2: Game speed 2x");
			UI.print("Command+3: Game speed 3x");
			UI.print("Command+4: Game speed 4x");
			UI.print("Command+5: Game speed 5x");
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////

		public static void killAllEnemies() {
			AManager<UnitManager>.getInstance().killAllEnemies();
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		public static void letItRain() {
			Weather weather = SingletonMonoBehaviour.FindGameComponent<Weather>();
			weather.rainStorm(0, 0, weather.worldSize3i.x, weather.worldSize3i.z);
		}

		///////////////////////////////////////////////////////////////////////////////////////////		
		public static void setGameSpeed1() {
			AManager<TimeManager>.getInstance().play(1f);
			//UI.print("Game speed 1x");	
		}

		///////////////////////////////////////////////////////////////////////////////////////////		
		public static void setGameSpeed2() {
			AManager<TimeManager>.getInstance().play(2f);
			//UI.print("Game speed 2x");	
		}

		///////////////////////////////////////////////////////////////////////////////////////////		
		public static void setGameSpeed3() {
			AManager<TimeManager>.getInstance().play(3f);
			UI.print("Game speed 3x");	
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		public static void setGameSpeed4() {
			AManager<TimeManager>.getInstance().play(4f);
			UI.print("Game speed 4x");	
		}

		///////////////////////////////////////////////////////////////////////////////////////////		
		public static void setGameSpeed5() {
			AManager<TimeManager>.getInstance().play(5f);
			UI.print("Game speed 5x");	
		}

		///////////////////////////////////////////////////////////////////////////////////////////

		public override void Awake() {
			Dbg.trc(Dbg.Grp.Init, 3);
		}
		
		public void Start() {
			Dbg.trc(Dbg.Grp.Startup, 3);
			
			string str;
			str = "Keyboard commands active. Press ";
			foreach(KeyCode key in cmdCombo) {
				str += (key.ToString() + "+");
			}
			str += "H for help.";
			Dbg.printMsg(str);
		}

		public void OnGUI() {

			Event evt = Event.current;
			if((Event.current.type == EventType.KeyDown) && evt.isKey && checkPressed(cmdCombo)) {
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
				if(!UnityEngine.Input.GetKey(key)) {
					return false;
				}
			}
			return true;
		}

		private bool checkReleased(KeyCode[] combo) {
			foreach(KeyCode key in combo) {
				if(UnityEngine.Input.GetKey(key)) {
					return false;
				}
			}
			return true;
		}
	}

	///////////////////////////////////////////////////////////////////////////////////////////

		
}

