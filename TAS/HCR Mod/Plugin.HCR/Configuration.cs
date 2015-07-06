
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

[assembly: AssemblyVersion("0.3.22.*")] 

namespace Plugin.HCR {
	public class Configuration {

		public string confName = "HCR";
		public Version version = new Version("0.3.22");
		public Version build = new Version(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

		//file path relative to T&S.exe		
		public string filePathPrefix = "saves";

		//enable/disable components
		public ConfigValue isEnabledWeatherEffects = new ConfigValue(1);
		public ConfigValue isEnabledShowRainBlocks = new ConfigValue(1);
		public ConfigValue isEnabledInvasionConfig = new ConfigValue(1);
		public ConfigValue isEnabledImproveUnitTraits = new ConfigValue(1);
		public ConfigValue isEnabledMoreImmigrants = new ConfigValue(1);
		public ConfigValue isEnabledMoreMerchants = new ConfigValue(1);
		public ConfigValue isEnabledCheats = new ConfigValue(1);
		public ConfigValue isEnabledKeyboardCommands = new ConfigValue(1);		
		
		//;block invasions until day x..
		public ConfigValue noSpidersTillDay = new ConfigValue(7);
		public ConfigValue noSkeletonsTillDay = new ConfigValue(14);
		public ConfigValue noWolvesTillDay = new ConfigValue(14);
		public ConfigValue noGoblinsTillDay = new ConfigValue(14);
		public ConfigValue noNecromancersTillDay = new ConfigValue(30);

		//;..or replace invasion monsters if they come before the time defined
		//careful not define something circular, happened to me heh
		public ConfigValue replaceMonsters = new ConfigValue(1);
		//;1 = goblins,2 = skeletons, 3 = spiders, 4 = wolves, 5 = necro
		public ConfigValue replaceWolvesWith = new ConfigValue(3);
		public ConfigValue replaceSkeletonsWith = new ConfigValue(4);
		public ConfigValue replaceGoblinsWith = new ConfigValue(4);
		public ConfigValue replaceNecromancersWith = new ConfigValue(1);

		//set tracked Resources. selects all between IdxFirst und IdxLast
		//this will activate only when a new game is started
		public ConfigValue trackResourcesIdxFirst = new ConfigValue(1);
		public ConfigValue trackResourcesIdxLast = new ConfigValue(79);	//60+ is tools, 90+ is weapons

		//you have to enable TRACE_ON in Dbg.cs if you want to use these..
		//debugLevel 0 = off, 1 = all output ... n = less output		
		public ConfigValue isEnabledDebugLevel = new ConfigValue(1);
		//use win32 DebugOutputString instead of game text window
		public ConfigValue IsEnabledDebugOutputString = new ConfigValue(1);
		//logfile
		public ConfigValue IsEnabledDebugLogFile = new ConfigValue(1);
		//this is by no means a strict classification, see source code what's used where  
		public ConfigValue isEnabledDebugGroup = new ConfigValue((int)(
			Dbg.Grp.Init | Dbg.Grp.Startup | Dbg.Grp.Unity | Dbg.Grp.Time | Dbg.Grp.Terrain | Dbg.Grp.Weather | Dbg.Grp.Units | Dbg.Grp.Invasion
		));
		
		private IniFile iniFile;
		
		private static Configuration instance = new Configuration();			
		public static Configuration getInstance() {
			return instance; 
		}
		
		public bool init() {

			iniFile = new IniFile(filePathPrefix + "/" + confName + ".ini");
			if(!load()) {
				Dbg.printErr("Could not read ini file, rename/delete yours to start with default configuration");
				return false;	
			}
			return true;
		}
		
		private bool load() {
			try {
				string iniVersionStr;
				Version iniVersion;
				iniVersionStr = iniFile.read(confName, confName);
				if(iniVersionStr == "") {
					Dbg.printErr("Ini file not found or corrupt, creating new file");
					save();
					return true;
				}
				iniVersion = new Version(iniVersionStr);
				if(iniVersion < version) {
					Dbg.printErr("Old version ini file detected, can not proceed");
					return false;
				}
	
				FieldInfo[] fl = this.GetType().GetFields();
				foreach(FieldInfo fi in fl) {
					if(fi.FieldType.ToString().Contains("ConfigValue")) {
						//GUIManager gm = AManager<GUIManager>.getInstance();
						//UI.print("fi.FieldType : "+fi.FieldType.ToString());
						//UI.print("fi.Name "+fi.Name.ToString());
						FieldInfo[] flCv = fi.FieldType.GetFields();
						object configValue = fi.GetValue(this);
						foreach(FieldInfo fiCv in flCv) {
							object value = fiCv.GetValue(configValue);
							if(fiCv.Name.ToString().Contains("_value")) {
								string str = iniFile.read(fi.Name.ToString(), confName);
								if(str == "") {
									throw new ArgumentException(fi.Name.ToString());
								}
								int num;
								try {
									num = Convert.ToInt32(str, 10);
								} catch {
									fiCv.SetValue(configValue, str);
									continue;								
								}
								//TODO: whats this? fiCv.SetValue(configValue, num);							
							}
						}
					}
				}
				return true;
			} catch(Exception e) {
				Dbg.dumpExc(e);
				Dbg.printErr("Error reading ini file, bailing out");
				return false;
			}
		}
		
		private void save() {
			iniFile.write(confName, version.ToString(), confName);
			FieldInfo[] fl = this.GetType().GetFields();
			foreach(FieldInfo fi in fl) {
				if(fi.FieldType.ToString().Contains("ConfigValue")) {
					FieldInfo[] flCv = fi.FieldType.GetFields();
					object configValue = fi.GetValue(this);
					foreach(FieldInfo fiCv in flCv) {
						object value = fiCv.GetValue(configValue);
						if(fiCv.Name.ToString().Contains("_value")) {
							iniFile.write(fi.Name.ToString(), fiCv.GetValue(configValue).ToString(), confName);
						}
					}
				}
			}
		}
		
	}		
	
	public class ConfigValue {
		public object _value;
		
		public ConfigValue(string value) {
			_value = value;
		}

		public ConfigValue(int value) {
			_value = value;
		}
		
		public void set(int value) {
			_value = value;
		}
		
		public void set(string value) {
			_value = value;
		}
		
		public int get() {
			if((_value as int?) != null) {
				return (int)_value;
			}
			throw new InvalidCastException();
		}
		
		public bool getBool() {
			if((_value as int?) != null) {
				return (((int)_value) != 0);				
			}
			throw new InvalidCastException();
		}
		
		public string getStr() {
			if((_value as string) != null) {
				return (string)_value;
			}
			throw new InvalidCastException();
		}
		
		public string toEnabledString() {
			if(get() != 0) {
				return " enabled";
			} else {
				return " disabled";
			}
		}
		
	}
}

