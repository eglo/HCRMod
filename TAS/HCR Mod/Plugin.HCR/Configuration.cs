
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Plugin.HCR {
	public class Configuration {

		public string confName = "HCR";
		public string version = "0.2.16";
		
		public ConfigValue isEnabledWeatherEffects = new ConfigValue(1);
		public ConfigValue isEnabledShowRainBlocks = new ConfigValue(1);
		public ConfigValue isEnabledInvasionConfig = new ConfigValue(1);
		public ConfigValue isEnabledImproveUnitTraits = new ConfigValue(1);
		public ConfigValue isEnabledMoreImmigrants = new ConfigValue(1);
		public ConfigValue isEnabledKeyboardCommands = new ConfigValue(1);		
		
		//;block invasions until day x..
		public ConfigValue noSpidersTillDay = new ConfigValue(7);
		public ConfigValue noSkeletonsTillDay = new ConfigValue(14);
		public ConfigValue noWolvesTillDay = new ConfigValue(14);
		public ConfigValue noGoblinsTillDay = new ConfigValue(14);
		public ConfigValue noNecromancersTillDay = new ConfigValue(30);
		//;..or replace invasion monsters if they come before the time defined
		public ConfigValue replaceMonsters = new ConfigValue(1);
		//;1 = goblins,2 = skeletons, 3 = spiders, 4 = wolves, 5 = necro
		public ConfigValue replaceWolvesWith = new ConfigValue(3);
		public ConfigValue replaceSkeletonsWith = new ConfigValue(4);
		public ConfigValue replaceGoblinsWith = new ConfigValue(4);
		public ConfigValue replaceNecromancersWith = new ConfigValue(1);

		public ConfigValue isEnabledDebugLevel= new ConfigValue(4);
		public ConfigValue isEnabledDebugGroup= new ConfigValue((int)(
			Dbg.Grp.Init|Dbg.Grp.Startup|Dbg.Grp.Unity|Dbg.Grp.Time|Dbg.Grp.Map|Dbg.Grp.Weather|Dbg.Grp.Units|Dbg.Grp.Invasion
		));
		
		private IniFile iniFile;
		
		private static Configuration instance = new Configuration();			
		public static Configuration getInstance() {
			return instance; 
		}
		
		public void init() {
			iniFile = new IniFile("saves\\"+confName+".ini");
			if(!load())
				save();
		}
		
		private bool load() {
			
			FieldInfo[] fl = this.GetType().GetFields();
			foreach (FieldInfo fi in fl) {
				if (fi.FieldType.ToString().Contains("ConfigValue")) {
					//GUIManager gm = AManager<GUIManager>.getInstance();
					//UI.print("fi.FieldType : "+fi.FieldType.ToString());
					//UI.print("fi.Name "+fi.Name.ToString());
					FieldInfo[] flCv = fi.FieldType.GetFields();
					object configValue = fi.GetValue(this);
					foreach (FieldInfo fiCv in flCv) {
						object value = fiCv.GetValue(configValue);
						if (fiCv.Name.ToString().Contains("_value")) {
							//UI.print("fiCv.GetValue "+fiCv.GetValue(configValue).ToString());
						}
					}
				}
			}
			return false;
		}
		
		private void save() {
			Configuration conf = Configuration.getInstance();
			iniFile.write(conf.confName,conf.version,conf.confName);
			FieldInfo[] fl = this.GetType().GetFields();
			foreach (FieldInfo fi in fl) {
				if (fi.FieldType.ToString().Contains("ConfigValue")) {
					FieldInfo[] flCv = fi.FieldType.GetFields();
					object configValue = fi.GetValue(this);
					foreach (FieldInfo fiCv in flCv) {
						object value = fiCv.GetValue(configValue);
						if (fiCv.Name.ToString().Contains("_value")) {
							iniFile.write(fi.Name.ToString(),fiCv.GetValue(configValue).ToString(),conf.confName);
						}
					}
				}
			}
		}
		
	}		
	
	public class ConfigValue {
		public object _value;
		
		public ConfigValue(int value) {
			_value = value;
		}
		
		public void set(int value) {
			_value = value;
		}
		
		public int get() {
			if((_value as int?) != null) {
				return (int) _value;
			}
			throw new InvalidCastException();
		}
		
		public bool getBool() {
			if((_value as int?) != null) {
				return ((int) _value != 0);				
			}
			throw new InvalidCastException();
		}
		
		public string getStr() {
			if((_value as string) != null) {
				return (string) _value;
			}
			throw new InvalidCastException();
		}
		
		public string toEnabledString() {
			if (get() != 0) {
				return " enabled";
			} else {
				return " disabled";
			}
		}
		
	}
}

