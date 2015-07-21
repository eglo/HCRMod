#if !HCRDEBUG
#warning Trace info not active
#endif

#undef USE_STACKFRAMES_FOR_TRACE

//(?<sta>\s*Dbg.+\,\s*)(c)(?<end>\,.*\);)
//${sta}5${end}

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Plugin.HCR {

	
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
			Else = 2048,
			All = -1
		};

		private static Configuration conf = Configuration.getInstance();
		private static StreamWriter sw = null;		
		
		//*****************************************************************************************		

		public static void print(string inStr, bool isUIMsg = false) {
			string str = conf.confName + ":DBG:" + inStr;
			if(conf.IsEnabledDebugOutputString.getBool()) {
				OutputDebugString(str);
			} else {
				if(!isUIMsg) {
					UI.print(str);
				}
			}			
			if(conf.IsEnabledDebugLogFile.getBool()) {
				log(str);
			}			
		}

		//*****************************************************************************************		

		private static void log(string str) {
			
			if(conf.IsEnabledDebugLogFile.getBool()) {
				try {
					if(sw == null) {
						sw = new StreamWriter(conf.filePathPrefix + "/" + conf.confName + ".log");
						sw.AutoFlush = true;
					}
					sw.WriteLine(str);
				} catch {
					UI.print(conf.confName + ":Exception: Couldn't create/write to log file");
				}
			}			
		}
		
		//*****************************************************************************************		
		[Conditional("HCRDEBUG")]
		public static void msg(Dbg.Grp group, int dbgLvl, string str) {
			if(((conf.isEnabledDebugGroup.get() & (int)group) == 0) || (conf.isEnabledDebugLevel.get() == 0) || (dbgLvl < conf.isEnabledDebugLevel.get())) {
				return;
			}
			
			print("MSG:" + group.ToString() + ": " + str);
		}
		
		//*****************************************************************************************		
		[Conditional("HCRDEBUG")]
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
		
		//*****************************************************************************************		
		[Conditional("HCRDEBUG")]		
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

		
#if USE_STACKFRAMES_FOR_TRACE			
		//*****************************************************************************************		
		[Conditional("HCRDEBUG")]
		public static void trc(Dbg.Grp group, int dbgLvl, string str = "") {
			if(((conf.isEnabledDebugGroup.get() & (int)group) == 0) || (conf.isEnabledDebugLevel.get() == 0) || (dbgLvl < conf.isEnabledDebugLevel.get())) {
				return;
			}
			
			StackTrace st = new StackTrace();
			StackFrame[] frames = st.GetFrames();
			print("TRC:" + group.ToString() + ": " + frames[1].GetMethod().DeclaringType.ToString() + "." + frames[1].GetMethod().Name + ": " + str);
		}
#else
		//*****************************************************************************************		
		[Conditional("HCRDEBUG")]
		public static void trc(
			Dbg.Grp group, 
			int dbgLvl, 
			string str = "",
			[CallerMemberName] string memberName = "",
			[CallerFilePath] string sourceFilePath = "",
			[CallerLineNumber] int sourceLineNumber = 0
		) {
			if(((conf.isEnabledDebugGroup.get() & (int)group) == 0) || (conf.isEnabledDebugLevel.get() == 0) || (dbgLvl < conf.isEnabledDebugLevel.get())) {
				return;
			}
			string remove = "d:\\data\\projects\\TAS\\HCR Mod\\Plugin.HCR\\";			
			string source = sourceFilePath.Remove(sourceFilePath.IndexOf(remove),remove.Length);
			print("TRC:" + group.ToString() + ": " + source + ":" + memberName + ":" + sourceLineNumber + ": " + str);			
		}
#endif
		//*****************************************************************************************		
		[Conditional("HCRDEBUG")]
		public static void trcCaller(Dbg.Grp group, int dbgLvl, string str = "") {
			if(((conf.isEnabledDebugGroup.get() & (int)group) == 0) || (conf.isEnabledDebugLevel.get() == 0) || (dbgLvl < conf.isEnabledDebugLevel.get())) {
				return;
			}
			
			StackTrace st = new StackTrace();
			StackFrame[] frames = st.GetFrames();
			print("TRC:" + group.ToString() + ": " + frames[2].GetMethod().DeclaringType.ToString() + "." + frames[2].GetMethod().Name + ": " + str);
		}
		
		//*****************************************************************************************		
		public static void dumpExc(Exception e) {
			print("EXC:" + e.ToString());
			StackTrace st = new StackTrace(e);
			StackFrame[] frames = st.GetFrames();
			foreach(StackFrame frame in frames) {
				//print(" in: " + frame.GetMethod().Name + " at line "+frame.GetFileLineNumber().ToString());
				print(" in: " + frame.GetMethod().DeclaringType.ToString() + "." + frame.GetMethod().Name);
			}
		}
		
		//*****************************************************************************************		
		public static void dumpCorExc(string corName, Exception e) {
			print("COREXC:" + corName + ": " + e.ToString());
			StackTrace st = new StackTrace(e);
			StackFrame[] frames = st.GetFrames();
			foreach(StackFrame frame in frames) {
				//print(" in: " + frame.GetMethod().Name + " at line "+frame.GetFileLineNumber().ToString());
				print(" in: " + frame.GetMethod().DeclaringType.ToString() + "." + frame.GetMethod().Name);
			}
		}

		//*****************************************************************************************		
		public static void printMsg(string str) {
			print("MSG:" + str);
		}		
		
		//*****************************************************************************************		
		public static void printErr(string str) {
			//print this to game text window
			UI.print(conf.confName + ":ERROR: " + str);
		}

	}
}

