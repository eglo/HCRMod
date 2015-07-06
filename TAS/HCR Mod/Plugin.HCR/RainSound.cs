using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timber_and_Stone;


namespace Plugin.HCR {


	public class Fader : SingletonMonoBehaviour {

		public static AudioSource asrc;		
	
		public override void Awake() {
			Setup();
		}
		
		public void Start() {
			Dbg.trc(Dbg.Grp.Startup, 3, "Fader started");
			asrc = transform.parent.GetComponent<AudioSource>();			
		}

		void FaderDoFade(bool up) {
			StartCoroutine(rainSoundFade(up));
		}

		IEnumerator rainSoundFade(bool up) {
			yield return new WaitForSeconds(0.1f);
			if(up) {
				asrc.volume += 0.05f;
				Dbg.trc(Dbg.Grp.Rain, 3, "up");				
				if(asrc.volume >= 0.9) {
					yield break;
				}					
			} else {
				Dbg.trc(Dbg.Grp.Rain, 3, "down");				
				asrc.volume -= 0.05f;
				if(asrc.volume <= 0.1) {
					asrc.Pause();
					yield break;
				}				
			}
			yield return new WaitForSeconds(0.1f);
		}		
		
	}
	
	public class RainSound : SingletonMonoBehaviour {
		private static string filePath = "file:///" + Application.dataPath + "/StreamingAssets/thunderstorm.ogg";
		private static WWW www = new WWW(filePath);
		public static AudioSource asrc;		
		
		public override void Awake() {
			Setup();
		}
		
		public void Start() {
			Dbg.trc(Dbg.Grp.Startup, 3, "RainSound started");
			Dbg.msg(Dbg.Grp.Rain, 3, "Using rain sound file:" + filePath);
			asrc = (AudioSource)gameObject.AddComponent<AudioSource>();
			AddGameComponent<Fader>();
			StartCoroutine(rainSoundLoad());
		}
				
		IEnumerator rainSoundLoad() {
			yield return www;
			if(www.error != null) {
				Dbg.trc(Dbg.Grp.Rain, 3, www.error);
			}
			
			asrc.clip = www.GetAudioClip(false, false);
			if(asrc.clip == null) {
				Dbg.trc(Dbg.Grp.Rain, 3, "clip == null");
			} else {
				Dbg.trc(Dbg.Grp.Rain, 3, "clip == " + asrc.clip.ToString());
			}
			Dbg.trc(Dbg.Grp.Rain, 3, "done");
		}
		
		public void rainSoundPlay() {
			Dbg.trc(Dbg.Grp.Rain, 3, asrc.ToString());
			
			if(!asrc.isPlaying && asrc.clip.isReadyToPlay) {
				asrc.loop = true;
				asrc.Play();
				SendMessage("FaderDoFade", (object)true);
			}
		}
		
		public void rainSoundStop() {
			Dbg.trc(Dbg.Grp.Rain, 3, asrc.ToString());
			SendMessage("FaderDoFade", (object)false);
		}


	}	
}

