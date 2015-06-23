
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Media;

namespace Plugin.HCR {
	public class Display {
		private static Configuration conf = Configuration.getInstance();
		private static GUIManager gm = AManager<GUIManager>.getInstance();
		
		public static void printDebug(int debugLevel,string str) {
			if ((conf.isEnabledDebugLevel.get() == 0) || (debugLevel < conf.isEnabledDebugLevel.get()))
				return;
			
			gm.AddTextLine(conf.confName+":DBG: "+str);
		}
		
		public static void printTrace(int traceLevel, int position) {
			if ((conf.isEnabledTraceLevel.get() == 0) || (traceLevel < conf.isEnabledTraceLevel.get()))
				return;
			
			GUIManager gm = AManager<GUIManager>.getInstance();
			StackTrace st = new StackTrace();
			StackFrame[] frames = st.GetFrames();
			gm.AddTextLine(conf.confName+":TRC: " + frames[1].GetMethod().Name+": "+position.ToString());
			
		}
		
		public static void printTrace(int traceLevel, string str = "") {
			if ((conf.isEnabledTraceLevel.get() == 0) || (traceLevel < conf.isEnabledTraceLevel.get()))
				return;
			
			GUIManager gm = AManager<GUIManager>.getInstance();
			StackTrace st = new StackTrace();
			StackFrame[] frames = st.GetFrames();
			gm.AddTextLine(conf.confName+":TRC: " + frames[1].GetMethod().Name+": "+str);
		}
		
		public static void printException(Exception e) {
			GUIManager gm = AManager<GUIManager>.getInstance();
			gm.AddTextLine(conf.confName+":EXC: " + e.ToString());
			StackTrace st = new StackTrace(e);
			StackFrame[] frames = st.GetFrames();
			foreach(StackFrame frame in frames) {
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

