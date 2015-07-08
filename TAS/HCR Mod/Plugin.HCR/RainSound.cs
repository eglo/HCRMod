using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timber_and_Stone;


namespace Plugin.HCR {


	public class Fader : SingletonMonoBehaviour {

		private RainSound rs;
		public AudioSource asrc;		
	
		public override void Awake() {
		}
		
		public void Start() {
			Dbg.trc(Dbg.Grp.Startup, 3, "Fader started");

			//not valid yet, bah
//			RainSound rs = gameObject.GetComponent<RainSound>();
//			asrc = rs.asrc;
//			Dbg.trc(Dbg.Grp.Startup|Dbg.Grp.Sound, 3,"Audiosource linked: "+asrc.gameObject.name+":"+asrc.name);
//			Dbg.trc(Dbg.Grp.Startup|Dbg.Grp.Sound, 3, "clip: " + asrc.clip.ToString()+":" + asrc.clip.name);
			
		}

		public void FaderDoFade(bool dir) {
			try {
				Dbg.trc(Dbg.Grp.Sound,3, dir.ToString());
	
				Dbg.trc(Dbg.Grp.Sound, 3,"go= "+go.ToString());
				Dbg.trc(Dbg.Grp.Sound, 3,"go.gameobj= "+go.gameObject.ToString());
				Dbg.trc(Dbg.Grp.Sound, 3,"gameobj= "+gameObject.ToString());
				Dbg.trc(Dbg.Grp.Sound, 3,"gameobj.gameobj= "+gameObject.gameObject.ToString());
				Dbg.trc(Dbg.Grp.Sound, 3,"gameobj.gameobj.gameobj= "+gameObject.gameObject.gameObject.ToString());
				Dbg.trc(Dbg.Grp.Sound, 3,"go.gameobj.gameobj= "+go.gameObject.gameObject.ToString());
				Dbg.trc(Dbg.Grp.Sound, 3,"transform.parent= "+transform.parent.ToString());
				
				rs = transform.parent.GetComponent<RainSound>();
				Dbg.trc(Dbg.Grp.Sound, 3,"rs= "+rs.ToString());
				asrc = rs.asrc;

				Dbg.trc(Dbg.Grp.Sound, 3,"Audiosource linked: "+asrc.gameObject.name+":"+asrc.name);
				Dbg.trc(Dbg.Grp.Sound, 3, "clip: " + asrc.clip.ToString()+":" + asrc.clip.name);
				
				StartCoroutine(rainSoundFade(dir));
			} catch(Exception e) { 
				Dbg.dumpExc(e);
			}
		}

		IEnumerator rainSoundFade(bool up) {
			while (true) {
				Dbg.trc(Dbg.Grp.Sound, 3, "asrc.volume: " + asrc.volume.ToString());
				if(up) {
					asrc.volume += 0.05f;
					Dbg.trc(Dbg.Grp.Rain, 3, "up");				
					if(asrc.volume >= 1.0) {
						Dbg.trc(Dbg.Grp.Sound, 3, "asrc.volume max: " + asrc.volume.ToString());
						asrc.volume = 1.0f;
						yield break;
					}					
				} else {
					Dbg.trc(Dbg.Grp.Rain, 3, "down");				
					asrc.volume -= 0.05f;
					if(asrc.volume <= 0.0) {
						asrc.volume = 0.0f;
						asrc.Pause();
						Dbg.trc(Dbg.Grp.Sound, 3, "asrc.volume off: " + asrc.volume.ToString());
						yield break;
					}				
				}
				Dbg.trc(Dbg.Grp.Sound, 3, "asrc.volume: " + asrc.volume.ToString());
				yield return new WaitForSeconds(0.2f);
			}
		}		
	}
	
	public class RainSound : SingletonMonoBehaviour {
		private static string filePath = "file:///" + Application.dataPath + "/StreamingAssets/thunderstorm.ogg";
		private static WWW www = new WWW(filePath);
		public Fader fader;
		public AudioSource asrc;		
		
		public override void Awake() {
			Dbg.trc(Dbg.Grp.Init, 3);
		}
		
		public void Start() {
			Dbg.trc(Dbg.Grp.Startup, 3);

			asrc = (AudioSource)go.AddComponent<AudioSource>();
			Dbg.trc(Dbg.Grp.Startup, 3,"Audiosource added: "+asrc.gameObject.name+":"+asrc.name);
			fader = AddGameComponent<Fader>(this.transform);
			
			Dbg.msg(Dbg.Grp.Startup, 3, "Using rain sound file:" + filePath);			
			StartCoroutine(rainSoundLoad());
		}
				
		IEnumerator rainSoundLoad() {
			yield return www;
			if(www.error != null) {
				Dbg.trc(Dbg.Grp.Sound, 3, www.error);
			}
			
			asrc.clip = www.GetAudioClip(false, false);
			if(asrc.clip == null) {
				Dbg.trc(Dbg.Grp.Sound, 3, "clip = null");
			} else {
				Dbg.trc(Dbg.Grp.Sound, 3, "clip: " + asrc.clip.ToString()+":" + asrc.clip.name);
			}
			Dbg.trc(Dbg.Grp.Sound, 3, "done");
		}
		
		public void rainSoundPlay() {
			Dbg.trc(Dbg.Grp.Rain, 3, asrc.ToString());
			
			if(!asrc.isPlaying && asrc.clip.isReadyToPlay) {
				asrc.loop = true;
				asrc.volume = 0;
				asrc.Play();
				fader.SendMessage("FaderDoFade",(object)true);
			}
		}
		
		public void rainSoundStop() {
			Dbg.trc(Dbg.Grp.Rain, 3, asrc.ToString());
			fader.SendMessage("FaderDoFade",(object)false);			
		}
	}	
}

