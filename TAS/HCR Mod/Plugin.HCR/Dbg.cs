
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Media;

namespace Plugin.HCR {

	
	public class Dbg {

		[Flags] public enum Grp {
			Init = 1, 
			Startup = 2, 
			Unity = 4, 
			Time = 8, 
			Map = 16, 
			Weather = 32,
			Rain = 64,
			Units = 128, 
			Invasion = 256
		};

		private static Configuration conf = Configuration.getInstance();
		private static GUIManager gm = AManager<GUIManager>.getInstance();

		public static void msg(Dbg.Grp group, int dbgLvl,string str) {
			if (((conf.isEnabledDebugGroup.get() & (int)group) == 0) || (conf.isEnabledDebugLevel.get() == 0) || (dbgLvl < conf.isEnabledDebugLevel.get()))
				return;
			
			gm.AddTextLine(conf.confName+":DBG:"+group.ToString()+": "+str);
		}
		
		public static void msg(Dbg.Grp group, int dbgLvl,string str, params int[] parms) {
			if (((conf.isEnabledDebugGroup.get() & (int)group) == 0) || (conf.isEnabledDebugLevel.get() == 0) || (dbgLvl < conf.isEnabledDebugLevel.get()))
				return;
			
			string outStr = str;
			foreach (int prm in parms) {
				outStr += prm.ToString()+", ";
			} 
			gm.AddTextLine(conf.confName+":DBG:"+group.ToString()+": "+outStr);
		}
		
		public static void msg(Dbg.Grp group, int dbgLvl,string str, params object[] parms) {
			if (((conf.isEnabledDebugGroup.get() & (int)group) == 0) || (conf.isEnabledDebugLevel.get() == 0) || (dbgLvl < conf.isEnabledDebugLevel.get()))
				return;
			
			string outStr = str;
			foreach (object prm in parms) {
				outStr += prm.ToString()+", ";
			} 
			gm.AddTextLine(conf.confName+":DBG:"+group.ToString()+": "+outStr);
		}

		public static void trc(Dbg.Grp group, int dbgLvl, int position) {
			if (((conf.isEnabledDebugGroup.get() & (int)group) == 0) || (conf.isEnabledDebugLevel.get() == 0) || (dbgLvl < conf.isEnabledDebugLevel.get()))
				return;
			
			GUIManager gm = AManager<GUIManager>.getInstance();
			StackTrace st = new StackTrace();
			StackFrame[] frames = st.GetFrames();
			gm.AddTextLine(conf.confName+":TRC:"+group.ToString()+": " + frames[1].GetMethod().Name+": "+position.ToString());
			
		}
		
		public static void trc(Dbg.Grp group, int dbgLvl, string str = "") {
			if (((conf.isEnabledDebugGroup.get() & (int)group) == 0) || (conf.isEnabledDebugLevel.get() == 0) || (dbgLvl < conf.isEnabledDebugLevel.get()))
				return;
			
			GUIManager gm = AManager<GUIManager>.getInstance();
			StackTrace st = new StackTrace();
			StackFrame[] frames = st.GetFrames();
			gm.AddTextLine(conf.confName+":TRC:"+group.ToString()+": " + frames[1].GetMethod().Name+": "+str);
		}
		
		public static void dumpExc(Exception e) {
			GUIManager gm = AManager<GUIManager>.getInstance();
			gm.AddTextLine(conf.confName+":EXC: " + e.ToString());
			StackTrace st = new StackTrace(e);
			StackFrame[] frames = st.GetFrames();
			foreach(StackFrame frame in frames) {
				//cant get line numbers with MonoDevelop .. :(
				//gm.AddTextLine(" in: " + frame.GetMethod().Name + " at line "+frame.GetFileLineNumber().ToString());
				gm.AddTextLine(" in: " + frame.GetMethod().Name);
			}
		}
		
		public static void dumpCorExc(string corName,Exception e) {
			GUIManager gm = AManager<GUIManager>.getInstance();
			gm.AddTextLine(conf.confName+":COREXC:" +corName+": " + e.ToString());
			StackTrace st = new StackTrace(e);
			StackFrame[] frames = st.GetFrames();
			foreach(StackFrame frame in frames) {
				//cant get line numbers with MonoDevelop .. :(
				//gm.AddTextLine(" in: " + frame.GetMethod().Name + " at line "+frame.GetFileLineNumber().ToString());
				gm.AddTextLine(" in: " + frame.GetMethod().Name);
			}
		}
		
		public static void printErr(string str) {
			GUIManager gm = AManager<GUIManager>.getInstance();
			gm.AddTextLine(conf.confName+":ERROR: " + str);
		}

		public static void printMsg(string str) {
			GUIManager gm = AManager<GUIManager>.getInstance();
			gm.AddTextLine(conf.confName+" "+str);
		}
		
	}
}

