using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Timber_and_Stone;


namespace Plugin.HCR {

	public class UnitTraits : Entity {

		public static Dictionary <string,int> unitProfessionLevels = new Dictionary <string,int>();
		bool isInitialized = false;
		
		//*****************************************************************************************

		public override void Awake() {
			Dbg.trc(Dbg.Grp.Init, 5);
		}
		

		public void Start() {
			if(!Configuration.getInstance().isEnabledImproveUnitTraits.getBool()) {
				return;
			}

			Dbg.trc(Dbg.Grp.Startup, 5, "Improve unit traits started");

			UnitManager um = AManager<UnitManager>.getInstance();
			if(um.playerUnits.Count == 0) {
				Dbg.printErr("UnitManager not initialized");
			} //farg..
			
			StartCoroutine(CheckLevelUp(2.0F));
		}
		
		//*****************************************************************************************

		IEnumerator CheckLevelUp(float waitTime) {
			UnitManager um = AManager<UnitManager>.getInstance();

			while(true) {
				yield return new WaitForSeconds(waitTime);
				while(um.playerUnits.Count == 0) {
					Dbg.trc(Dbg.Grp.Units, 3, "UnitManager not initialized");
					yield return new WaitForSeconds(waitTime);
				}
				try {
	
					if(!isInitialized) {
						foreach(APlayableEntity unit in um.playerUnits) {
							Dbg.trc(Dbg.Grp.Units, 3, "unit " + unit.unitName);
							foreach(AProfession prof in unit.getProfessions()) {
								string unitNameAndProfession = unit.unitName + " " + prof.getProfessionName();
								unitProfessionLevels[unitNameAndProfession] = prof.getLevel();
							}
						}
						isInitialized = true;			
					}

					foreach(APlayableEntity unit in um.playerUnits) {
						Dbg.trc(Dbg.Grp.Units, 3, "unit " + unit.unitName);
						foreach(AProfession prof in unit.getProfessions()) {
							string unitNameAndProfession = unit.unitName + " " + prof.getProfessionName();
							int lvl = prof.getLevel();
							int donelvl = 0;
							Dbg.trc(Dbg.Grp.Units, 3, "tryLvlUp " + unitNameAndProfession);
							if(unitProfessionLevels.TryGetValue(unitNameAndProfession, out donelvl)) {
								if(donelvl >= lvl) {
									continue;
								}
							} else {
								//either noob or name edit
								unitProfessionLevels[unitNameAndProfession] = prof.getLevel();
								continue;
							}
							unitProfessionLevels[unitNameAndProfession] = lvl;
							Dbg.trc(Dbg.Grp.Units, 4, "randomLvlUp " + unitNameAndProfession);
							if(UnityEngine.Random.Range(0, (20 - lvl)) == 0) {
								Dbg.trc(Dbg.Grp.Units, 5, "doLvlUp " + unitNameAndProfession);
								processLevelUp(unit.unitName, lvl);
							}
						}
					}	
					
					Dbg.trc(Dbg.Grp.Units, 3);
				} catch(Exception e) { 
					Dbg.dumpCorExc("CheckLevelUp", e);
				}
			}
		}
		
		//*****************************************************************************************
		// get unit <unitName> and either remove negative traits or apply positive ones if no negatives found 
		private void processLevelUp(string unitName, int lvl) {
			Dbg.trc(Dbg.Grp.Units, 5);
			
			UnitManager um = AManager<UnitManager>.getInstance();
			foreach(APlayableEntity unit in um.playerUnits) {
				if(unit.unitName == unitName) {
					if(!removeNegativeTrait(unit)) {
						addPositiveTrait(unit);
					}
					return;
				}
			}
			Dbg.printErr("Could not find unit " + unitName + " for improving traits");
		}
		
		//*****************************************************************************************
		//find all negative traits on unit and randomly remove one, return false if no negs found
		private bool removeNegativeTrait(APlayableEntity unit) {
			Dbg.trc(Dbg.Grp.Units, 5);
			
			List<string> negPrefs = new List<string> {"trait.weakback","trait.cowardly","trait.clumsy","trait.sluggish","trait.overeater","trait.disloyal","trait.badvision","trait.lazy"};
			List<string> hasPrefs = new List<string>();
			foreach(string pref in negPrefs) {
				if(unit.preferences[pref] == true) {
					hasPrefs.Add(pref);
				}
			}
			if(hasPrefs.Count >= 1) {
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
				int remove = UnityEngine.Random.Range(0, hasPrefs.Count - 1);
				unit.preferences[hasPrefs[remove]] = false;
				string outString;
				traitsChangedStrings.TryGetValue(hasPrefs[remove], out outString);
				UI.print("All this training paid off! " + unit.unitName + " no longer " + outString + "!");
				return true;
			}
			return false;
		}
		
		
		//*****************************************************************************************
		//add a new random postive trait to unit
		
		private bool addPositiveTrait(APlayableEntity unit) {
			Dbg.trc(Dbg.Grp.Units, 5);
			
			List<string> posPrefs = new List<string> {"trait.hardworker","trait.goodvision","trait.charismatic","trait.courageous","trait.athletic","trait.quicklearner","trait.strongback"};
			List<string> notHasPrefs = new List<string>();
			foreach(string pref in posPrefs) {
				if(unit.preferences[pref] != true) {
					notHasPrefs.Add(pref);
				}
			}
			if(notHasPrefs.Count >= 1) {
				Dictionary<string,string> traitsChangedStrings = new Dictionary<string,string>{
					{"trait.hardworker","is a hard worker"},
					{"trait.goodvision","has good vision"},
					{"trait.charismatic","is charismatic"},
					{"trait.courageous","is courageous"},
					{"trait.athletic","is athletic"},
					{"trait.quicklearner","is a quick learner"},
					{"trait.strongback","has a strong back"},
				};
				int add = UnityEngine.Random.Range(0, notHasPrefs.Count - 1);
				unit.preferences[notHasPrefs[add]] = true;
				string outString;
				traitsChangedStrings.TryGetValue(notHasPrefs[add], out outString);
				UI.print("All this training paid off! " + unit.unitName + " now " + outString + "!");
				return true;
			}			
			return false;
		}
	}
}

