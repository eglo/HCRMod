
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Timber_and_Stone;

namespace Plugin.HCR {
	public class ImproveUnitTraits : MonoBehaviour {

		private Dictionary <string,int> unitLevelUpEvents = new Dictionary <string,int>();
		private int ticks;
		
		public ImproveUnitTraits() {
		}

		///////////////////////////////////////////////////////////////////////////////////////////

		public void Start() {
			ticks = 0;
			Display.printDebug(5,"Improve unit traits started");		
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////

		public void Update() {
			try {
				ticks++;
				if ((Configuration.getInstance().isEnabledImproveUnitTraits.getBool()) && (ticks >= 128)) {
					//AManager<GUIManager>.getInstance().AddTextLine("Level Up! Long Dong Tom is now a Lv. 20 tester");
					unitCheckLevelUpEventUglyHack();
					ticks = 0;
				}
			} catch(Exception e) { 
				Display.printException(e);
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		// check the Notification Window for Level Up messages, when found get unit and randomly improve unit traits  
		private void unitCheckLevelUpEventUglyHack() {
			GUIManager gm = AManager<GUIManager>.getInstance();
			NotificationWindow nw = gm.GetComponent<NotificationWindow>();
			
			List<string> text;
			text = nw.textLines;
			for(int i = 0; i < text.Count; i++) {
				string line = text[i];
				if (!line.StartsWith("Level Up!"))
					continue;
				Match mc = Regex.Match(line,@"Level Up! (.+) is now a Lv. (\d+)\s(\w+)");
				if (mc.Success) {
					string unitName = mc.Groups[1].Value;
					int lvl = Int32.Parse(mc.Groups[2].Value);
					string profession = mc.Groups[3].Value;
					Display.printDebug(2,"unitName "+unitName);
					Display.printDebug(2,"lvl "+lvl.ToString());
					Display.printDebug(2,"profession "+profession);
					
					int donelvl;
					Display.printTrace(2,"tryLvlUp "+unitName+" "+profession);
					if (unitLevelUpEvents.TryGetValue(unitName+" "+profession,out donelvl) && (donelvl == lvl)) {
						//already done this guy
						return;
					}
					unitLevelUpEvents[unitName+" "+profession] = lvl;
					Display.printTrace(2,"randomLvlUp "+unitName+" "+profession);
					if(UnityEngine.Random.Range(0,(20-lvl)) == 0) {
						Display.printTrace(3,"doLvlUp "+unitName+" "+profession);
						unitLevelUp(unitName, lvl);
					}
				}
			}
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		// get unit <unitName> and either remove negative traits or apply positive ones if no negatives found 
		private void unitLevelUp(string unitName,int lvl) {
			Display.printTrace(3);
			
			UnitManager um = AManager<UnitManager>.getInstance();
			foreach (APlayableEntity unit in um.playerUnits) {
				if (unit.unitName == unitName) {
					if(!unitRemoveRandomNegativeTrait(unit)) 
						unitAddRandomPositiveTrait(unit);
					return;
				}
			}
			Display.printErr("Could not find unit "+unitName+" for improving traits");
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		//find all negative traits on unit and randomly remove one, retunr false if no negs found
		private bool unitRemoveRandomNegativeTrait(APlayableEntity unit) {
			Display.printTrace(3,1);
			
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
				GUIManager gm = AManager<GUIManager>.getInstance();
				string outString;
				traitsChangedStrings.TryGetValue(hasPrefs[remove],out outString);
				gm.AddTextLine("All this training paid off! "+unit.unitName+ " no longer "+outString+"!");
				return true;
			}
			return false;
		}
		
		
		///////////////////////////////////////////////////////////////////////////////////////////
		//add a new random postive trait to unit
		
		private bool unitAddRandomPositiveTrait(APlayableEntity unit) {
			Display.printTrace(3,1);
			
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
				GUIManager gm = AManager<GUIManager>.getInstance();
				string outString;
				traitsChangedStrings.TryGetValue(notHasPrefs[add],out outString);
				gm.AddTextLine("All this training paid off! "+unit.unitName+ " now "+outString+"!");
				return true;
			}
			
			return true;
		}
		
	}
}

