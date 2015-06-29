
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Plugin.HCR {

	public class UI {
		private static GUIManager gm = AManager<GUIManager>.getInstance();
		private static Configuration conf = Configuration.getInstance();
		private static StreamWriter sw;
				
		public static void print(string str) {
			gm.AddTextLine(str);

			if (conf.IsEnabledLogFile.getBool()) {
				try {
					if (sw == null) {
						sw = new StreamWriter("saves\\"+conf.confName+".log");
						sw.AutoFlush = true;
					}
					sw.WriteLine(str);
				} catch {
					gm.AddTextLine("HCR:Exception: Couldn't create/write to log file");
				}
			}
		}
	}
	
	public class Dbg {

		[Flags] public enum Grp {
			None = 0,
			Init = 1, 
			Startup = 2, 
			Unity = 4, 
			Time = 8, 
			Map = 16, 
			Weather = 32,
			Rain = 64,
			Units = 128, 
			Invasion = 256,
			All = -1
		};

		private static Configuration conf = Configuration.getInstance();

		public static void msg(Dbg.Grp group, int dbgLvl,string str) {
			if (((conf.isEnabledDebugGroup.get() & (int)group) == 0) || (conf.isEnabledDebugLevel.get() == 0) || (dbgLvl < conf.isEnabledDebugLevel.get()))
				return;
			
			UI.print(conf.confName+":DBG:"+group.ToString()+": "+str);
		}
		
		public static void msg(Dbg.Grp group, int dbgLvl,string str, params int[] parms) {
			if (((conf.isEnabledDebugGroup.get() & (int)group) == 0) || (conf.isEnabledDebugLevel.get() == 0) || (dbgLvl < conf.isEnabledDebugLevel.get()))
				return;
			
			string outStr = str;
			foreach (int prm in parms) {
				outStr += prm.ToString()+", ";
			} 
			UI.print(conf.confName+":DBG:"+group.ToString()+": "+outStr);
		}
		
		public static void msg(Dbg.Grp group, int dbgLvl,string str, params object[] parms) {
			if (((conf.isEnabledDebugGroup.get() & (int)group) == 0) || (conf.isEnabledDebugLevel.get() == 0) || (dbgLvl < conf.isEnabledDebugLevel.get()))
				return;
			
			string outStr = str;
			foreach (object prm in parms) {
				outStr += prm.ToString()+", ";
			} 
			UI.print(conf.confName+":DBG:"+group.ToString()+": "+outStr);
		}

		public static void trc(Dbg.Grp group, int dbgLvl, int position) {
			if (((conf.isEnabledDebugGroup.get() & (int)group) == 0) || (conf.isEnabledDebugLevel.get() == 0) || (dbgLvl < conf.isEnabledDebugLevel.get()))
				return;
			
			StackTrace st = new StackTrace();
			StackFrame[] frames = st.GetFrames();
			UI.print(conf.confName+":TRC:"+group.ToString()+": "+frames[1].GetMethod().DeclaringType.ToString()+"."+frames[1].GetMethod().Name+": "+position.ToString());
			
		}
		
		public static void trc(Dbg.Grp group, int dbgLvl, string str = "") {
			if (((conf.isEnabledDebugGroup.get() & (int)group) == 0) || (conf.isEnabledDebugLevel.get() == 0) || (dbgLvl < conf.isEnabledDebugLevel.get()))
				return;
			
			StackTrace st = new StackTrace();
			StackFrame[] frames = st.GetFrames();
			UI.print(conf.confName+":TRC:"+group.ToString()+": "+frames[1].GetMethod().DeclaringType.ToString()+"."+frames[1].GetMethod().Name+": "+str);
		}
		
		public static void dumpExc(Exception e) {
			UI.print(conf.confName+":EXC: " + e.ToString());
			StackTrace st = new StackTrace(e);
			StackFrame[] frames = st.GetFrames();
			foreach(StackFrame frame in frames) {
				//cant get line numbers with MonoDevelop .. :(
				//UI.print(" in: " + frame.GetMethod().Name + " at line "+frame.GetFileLineNumber().ToString());
				UI.print(" in: "+frame.GetMethod().DeclaringType.ToString()+"."+frame.GetMethod().Name);
			}
		}
		
		public static void dumpCorExc(string corName,Exception e) {
			UI.print(conf.confName+":COREXC:" +corName+": " + e.ToString());
			StackTrace st = new StackTrace(e);
			StackFrame[] frames = st.GetFrames();
			foreach(StackFrame frame in frames) {
				//cant get line numbers with MonoDevelop .. :(
				//UI.print(" in: " + frame.GetMethod().Name + " at line "+frame.GetFileLineNumber().ToString());
				UI.print(" in: "+frame.GetMethod().DeclaringType.ToString()+"."+frame.GetMethod().Name);
			}
		}
		
		public static void printErr(string str) {
			UI.print(conf.confName+":ERROR: " + str);
		}

		public static void printMsg(string str) {
			UI.print(conf.confName+" "+str);
		}		
	}
}

