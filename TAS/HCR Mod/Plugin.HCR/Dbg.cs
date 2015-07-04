#if !TRACE_ON
#warning Trace info not active
#endif

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Plugin.HCR {

	
	public class UI {
				
		private static Configuration conf = Configuration.getInstance();
		private static GUIManager gm = AManager<GUIManager>.getInstance();

		public static void print(string str) {

			gm.AddTextLine(str);
			//send it to debug out too
			if(conf.IsEnabledDebugOutputString.getBool()) {
				Dbg.friendPrint(str);
			}			
		}
	}
	
	public class Dbg {

		[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		public static extern void OutputDebugString(string message);
		
		[Flags]
		public enum Grp {
			None = 0,
			Init = 1, 
			Startup = 2, 
			Unity = 4,
			Sound = 8,
			Light = 16,
			Time = 32, 
			Terrain = 64, 
			Weather = 128,
			Rain = 256,
			Units = 512, 
			Invasion = 1024,
			All = -1
		}
		;

		private static GUIManager gm = AManager<GUIManager>.getInstance();
		private static Configuration conf = Configuration.getInstance();
		private static StreamWriter sw = null;
		
		//poor man's friend access, only use from UI.print
		public static void friendPrint(string str) {
			print(str, true);
		}

		
		private static void print(string inStr, bool isUIMsg = false) {
			Configuration conf = Configuration.getInstance();

			string str = conf.confName + ":DBG:" + inStr;

			if(conf.IsEnabledDebugOutputString.getBool()) {
				OutputDebugString(str);
			} else {
				if(!isUIMsg) {
					gm.AddTextLine(str);
				}
			}			
			if(conf.IsEnabledDebugLogFile.getBool()) {
				log(str);
			}			
		}

		private static void log(string str) {
			
			if(conf.IsEnabledDebugLogFile.getBool()) {
				try {
					if(sw == null) {
						sw = new StreamWriter("saves\\" + conf.confName + ".log");
						sw.AutoFlush = true;
					}
					sw.WriteLine(str);
				} catch {
					gm.AddTextLine(conf.confName + ":Exception: Couldn't create/write to log file");
				}
			}			
		}
		
		[Conditional("TRACE_ON")]
		public static void msg(Dbg.Grp group, int dbgLvl, string str) {
			if(((conf.isEnabledDebugGroup.get() & (int)group) == 0) || (conf.isEnabledDebugLevel.get() == 0) || (dbgLvl < conf.isEnabledDebugLevel.get())) {
				return;
			}
			
			print("MSG:" + group.ToString() + ": " + str);
		}
		
		[Conditional("TRACE_ON")]
		public static void msg(Dbg.Grp group, int dbgLvl, string str, params int[] parms) {
			if(((conf.isEnabledDebugGroup.get() & (int)group) == 0) || (conf.isEnabledDebugLevel.get() == 0) || (dbgLvl < conf.isEnabledDebugLevel.get())) {
				return;
			}
			
			string outStr = str;
			foreach(int prm in parms) {
				outStr += prm.ToString() + ", ";
			} 
			print("MSG:" + group.ToString() + ": " + outStr);
		}
		
		[Conditional("TRACE_ON")]		
		public static void msg(Dbg.Grp group, int dbgLvl, string str, params object[] parms) {
			if(((conf.isEnabledDebugGroup.get() & (int)group) == 0) || (conf.isEnabledDebugLevel.get() == 0) || (dbgLvl < conf.isEnabledDebugLevel.get())) {
				return;
			}
			
			string outStr = str;
			foreach(object prm in parms) {
				outStr += prm.ToString() + ", ";
			} 
			print("MSG:" + group.ToString() + ": " + outStr);
		}

		[Conditional("TRACE_ON")]
		public static void trc(Dbg.Grp group, int dbgLvl, int position) {
			if(((conf.isEnabledDebugGroup.get() & (int)group) == 0) || (conf.isEnabledDebugLevel.get() == 0) || (dbgLvl < conf.isEnabledDebugLevel.get())) {
				return;
			}
			
			StackTrace st = new StackTrace();
			StackFrame[] frames = st.GetFrames();
			print("TRC:" + group.ToString() + ": " + frames[1].GetMethod().DeclaringType.ToString() + "." + frames[1].GetMethod().Name + ": " + position.ToString());			
		}
		
		[Conditional("TRACE_ON")]
		public static void trc(Dbg.Grp group, int dbgLvl, string str = "") {
			if(((conf.isEnabledDebugGroup.get() & (int)group) == 0) || (conf.isEnabledDebugLevel.get() == 0) || (dbgLvl < conf.isEnabledDebugLevel.get())) {
				return;
			}
			
			StackTrace st = new StackTrace();
			StackFrame[] frames = st.GetFrames();
			print("TRC:" + group.ToString() + ": " + frames[1].GetMethod().DeclaringType.ToString() + "." + frames[1].GetMethod().Name + ": " + str);
		}
		
		public static void dumpExc(Exception e) {
			print("EXC:" + e.ToString());
			StackTrace st = new StackTrace(e);
			StackFrame[] frames = st.GetFrames();
			foreach(StackFrame frame in frames) {
				//print(" in: " + frame.GetMethod().Name + " at line "+frame.GetFileLineNumber().ToString());
				print(" in: " + frame.GetMethod().DeclaringType.ToString() + "." + frame.GetMethod().Name);
			}
		}
		
		public static void dumpCorExc(string corName, Exception e) {
			print("COREXC:" + corName + ": " + e.ToString());
			StackTrace st = new StackTrace(e);
			StackFrame[] frames = st.GetFrames();
			foreach(StackFrame frame in frames) {
				//print(" in: " + frame.GetMethod().Name + " at line "+frame.GetFileLineNumber().ToString());
				print(" in: " + frame.GetMethod().DeclaringType.ToString() + "." + frame.GetMethod().Name);
			}
		}

		public static void printMsg(string str) {
			print("MSG:" + str);
		}		
		
		public static void printErr(string str) {
			//print this to game text window
			UI.print(conf.confName + ":ERROR: " + str);
		}

	}
}

