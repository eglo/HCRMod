using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timber_and_Stone;


namespace Plugin.HCR {
	
	public class RainSound : SingletonMonoBehaviour<RainSound> {
		private static string filePath = "file:///" + Application.dataPath + "/StreamingAssets/rain.ogg";
		private static WWW www = new WWW(filePath);
		public static AudioSource asrc;		
		
		public override void Start() {
			Dbg.trc(Dbg.Grp.Startup, 3, "RainSound started");
			Dbg.msg(Dbg.Grp.Rain, 3, "Using rain sound file:" + filePath);
			asrc = (AudioSource)AddComponent<AudioSource>();
			StartCoroutine(rainSoundLoad());
		}
		
		public override void Update() {
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
				asrc.volume = 1.0f;
				asrc.loop = true;
				asrc.Play();
			}			
		}
		
		public void rainSoundStop() {
			Dbg.trc(Dbg.Grp.Rain, 3, asrc.ToString());
			asrc.Pause();
		}		
	}	
}

