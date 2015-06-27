using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Timber_and_Stone;


namespace Plugin.HCR {
	public class ImproveUnitTraits : MonoBehaviour {

		private static ImproveUnitTraits instance = new ImproveUnitTraits();			
		public static ImproveUnitTraits getInstance() {
			return instance; 
		}
		
		public static Dictionary <string,int> unitLevelUpEvents = new Dictionary <string,int>();
		
		///////////////////////////////////////////////////////////////////////////////////////////

		public void Start() {
			if (!Configuration.getInstance().isEnabledImproveUnitTraits.getBool())
				return;

			Dbg.msg(Dbg.Grp.Startup,3,"Improve unit traits started");
			StartCoroutine(CheckLevelUp(1.0F));
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////

		IEnumerator CheckLevelUp(float waitTime) {
			while(true) {
				yield return new WaitForSeconds(waitTime);
				try {
					//AManager<GUIManager>.getInstance().AddTextLine("Level Up! Long Dong Tom is now a Lv. 20 tester");
					checkLevelUpEventUglyHack();
				} catch(Exception e) { 
					Dbg.dumpCorExc("CheckLevelUp",e);
				}
			}
		}


		///////////////////////////////////////////////////////////////////////////////////////////
		// check the Notification Window for Level Up messages, when found get unit and randomly improve unit traits  
		private void checkLevelUpEventUglyHack() {
			GUIManager gm = AManager<GUIManager>.getInstance();
			NotificationWindow nw = gm.GetComponent<NotificationWindow>();
			
			List<string> text;
			text = nw.textLines;
			for(int i = text.Count-1; i >= 0; i--) {
				string line = text[i];
				if (!line.StartsWith("Level Up!"))
					continue;
				Match mc = Regex.Match(line,@"Level Up! (.+) is now a Lv. (\d+)\s(\w+)");
				if (mc.Success) {
					string unitName = mc.Groups[1].Value;
					int lvl = Int32.Parse(mc.Groups[2].Value);
					string profession = mc.Groups[3].Value;
					Dbg.msg(Dbg.Grp.Units,2,"unitName "+unitName);
					Dbg.msg(Dbg.Grp.Units,2,"lvl "+lvl.ToString());
					Dbg.msg(Dbg.Grp.Units,2,"profession "+profession);
					string unitNameAndProfession = unitName+" "+profession;
					int donelvl = 0;
					Dbg.trc(Dbg.Grp.Units,2,"tryLvlUp "+unitNameAndProfession);
					if (unitLevelUpEvents.TryGetValue(unitNameAndProfession,out donelvl)) {
						if (donelvl >= lvl)	//wtf?..
							continue;
					}
					unitLevelUpEvents[unitNameAndProfession] = lvl;
					Dbg.trc(Dbg.Grp.Units,2,"randomLvlUp "+unitNameAndProfession);
					if(UnityEngine.Random.Range(0,(20-lvl)) == 0) {
						Dbg.trc(Dbg.Grp.Units,3,"doLvlUp "+unitNameAndProfession);
						processLevelUp(unitName, lvl);
					}
				}
			}
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		// get unit <unitName> and either remove negative traits or apply positive ones if no negatives found 
		private void processLevelUp(string unitName,int lvl) {
			Dbg.trc(Dbg.Grp.Units,3);
			
			UnitManager um = AManager<UnitManager>.getInstance();
			foreach (APlayableEntity unit in um.playerUnits) {
				if (unit.unitName == unitName) {
					if(!removeNegativeTrait(unit)) {
						addPositiveTrait(unit);
					}
					return;
				}
			}
			Dbg.printErr("Could not find unit "+unitName+" for improving traits");
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		//find all negative traits on unit and randomly remove one, retunr false if no negs found
		private bool removeNegativeTrait(APlayableEntity unit) {
			Dbg.trc(Dbg.Grp.Units,3);
			
			List<string> negPrefs = new List<string> {"trait.weakback","trait.cowardly","trait.clumsy","trait.sluggish","trait.overeater","trait.disloyal","trait.badvision","trait.lazy"};
			List<string> hasPrefs = new List<string>();
			foreach (string pref in negPrefs) {
				if (unit.preferences[pref] == true) {
					hasPrefs.Add(pref);
				}
			}
			if (hasPrefs.Count >= 1) {
				Dictionary<string,string> traitsChangedStrings = new Dictionary<string,string>{
					{"trait.weakback","has a weak back"},
					{"trait.cowardly","is a coward"},
					{"trait.clumsy","is clumsy"},
					{"trait.sluggish","is sluggish"},
					{"trait.overeater","is an overeater"},
					{"trait.disloyal","is disloyal"},
					{"trait.badvision","has bad vision"},
					{"trait.lazy","is lazy"}
				};
				int remove = UnityEngine.Random.Range(0,hasPrefs.Count-1);
				unit.preferences[hasPrefs[remove]] = false;
				string outString;
				traitsChangedStrings.TryGetValue(hasPrefs[remove],out outString);
				UI.print("All this training paid off! "+unit.unitName+ " no longer "+outString+"!");
				return true;
			}
			return false;
		}
		
		
		///////////////////////////////////////////////////////////////////////////////////////////
		//add a new random postive trait to unit
		
		private bool addPositiveTrait(APlayableEntity unit) {
			Dbg.trc(Dbg.Grp.Units,3);
			
			List<string> posPrefs = new List<string> {"trait.hardworker","trait.goodvision","trait.charismatic","trait.courageous","trait.athletic","trait.quicklearner","trait.strongback"};
			List<string> notHasPrefs = new List<string>();
			foreach (string pref in posPrefs) {
				if (unit.preferences[pref] != true) {
					notHasPrefs.Add(pref);
				}
			}
			if (notHasPrefs.Count >= 1) {
				Dictionary<string,string> traitsChangedStrings = new Dictionary<string,string>{
					{"trait.hardworker","is a hard worker"},
					{"trait.goodvision","has good vision"},
					{"trait.charismatic","is charismatic"},
					{"trait.courageous","is courageous"},
					{"trait.athletic","is athletic"},
					{"trait.quicklearner","is a quick learner"},
					{"trait.strongback","has a strong back"},
				};
				int add = UnityEngine.Random.Range(0,notHasPrefs.Count-1);
				unit.preferences[notHasPrefs[add]] = true;
				string outString;
				traitsChangedStrings.TryGetValue(notHasPrefs[add],out outString);
				UI.print("All this training paid off! "+unit.unitName+ " now "+outString+"!");
				return true;
			}			
			return false;
		}
	}
}

