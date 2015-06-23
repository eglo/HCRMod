
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Plugin.HCR {
	public class IniFile {

		[DllImport("kernel32",CharSet=CharSet.Unicode)]
		static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
		[DllImport("kernel32",CharSet=CharSet.Unicode)]
		static extern int GetPrivateProfileString(string section, string key, string Default, StringBuilder retVal, int size, string filePath);
		
		string path;
		
		public IniFile(string iniFileName) {
			path = new FileInfo(iniFileName).FullName.ToString();
		}
		
		public string read(string key, string section) {
			var retVal = new StringBuilder(255);
			GetPrivateProfileString(section, key, "", retVal, 255, path);
			return retVal.ToString();
		}
		
		public void write(string key, string val, string section) {
			WritePrivateProfileString(section, key, val, path);
		}
		
		public bool keyExists(string key, string section = null) {
			return read(key, section).Length > 0;
		}
	}
}

