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

		public void OnDoFade(bool dir) {
			try {
				Dbg.trc(Dbg.Grp.Sound,3, dir.ToString());
					
				rs = transform.parent.GetComponent<RainSound>();
				Dbg.trc(Dbg.Grp.Sound, 3,"rs= "+rs.ToString());
				asrc = rs.asrcCurrent;

				Dbg.trc(Dbg.Grp.Sound, 3,"Audiosource linked: "+asrc.gameObject.name+":"+asrc.name);
				Dbg.trc(Dbg.Grp.Sound, 3, "clip: " + asrc.clip.ToString()+":" + asrc.clip.name);
				
				StartCoroutine(doFader(dir));
			} catch(Exception e) { 
				Dbg.dumpExc(e);
			}
		}

		IEnumerator doFader(bool up) {
			while (true) {
				Dbg.trc(Dbg.Grp.Sound, 3, "asrc.volume: " + asrc.volume.ToString());
				if(up) {
					asrc.volume += 0.01f;
					Dbg.trc(Dbg.Grp.Rain, 3, "up");				
					if(asrc.volume >= 0.99) {
						Dbg.trc(Dbg.Grp.Sound, 3, "asrc.volume max: " + asrc.volume.ToString());
						asrc.volume = 1.0f;
						yield break;
					}					
				} else {
					Dbg.trc(Dbg.Grp.Rain, 3, "down");				
					asrc.volume -= 0.01f;
					if(asrc.volume <= 0.01f) {
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
		private string applPath = "file:///" + Application.dataPath + "/../";
		public AudioSource asrcRainLight;		
		public AudioSource asrcRainHeavy;		
		public AudioSource asrcRainThunderstorm;		
		public AudioSource asrcCurrent;		
		public Fader fader;
		
		public override void Awake() {
			Dbg.trc(Dbg.Grp.Init, 3);
		}
		
		public void Start() {
			Dbg.trc(Dbg.Grp.Startup, 3);

			AudioSource asrc;
			asrc = asrcRainLight =  (AudioSource)go.AddComponent<AudioSource>();
			StartCoroutine(rainSoundLoad(asrcRainLight,"rainLight.ogg"));
			Dbg.trc(Dbg.Grp.Startup, 3,"Audiosource added: "+asrc.gameObject.name+":"+asrc.name);			

			asrc = asrcRainHeavy =  (AudioSource)go.AddComponent<AudioSource>();
			StartCoroutine(rainSoundLoad(asrcRainHeavy,"rainHeavy.ogg"));
			Dbg.trc(Dbg.Grp.Startup, 3,"Audiosource added: "+asrc.gameObject.name+":"+asrc.name);			
			
			asrc = asrcRainThunderstorm =  (AudioSource)go.AddComponent<AudioSource>();
			StartCoroutine(rainSoundLoad(asrcRainThunderstorm,"rainThunderstorm.ogg"));
			Dbg.trc(Dbg.Grp.Startup, 3,"Audiosource added: "+asrc.gameObject.name+":"+asrc.name);			

			asrcCurrent = asrcRainThunderstorm;
						
			fader = AddGameComponent<Fader>(this.transform);
		}
				

		IEnumerator rainSoundLoad(AudioSource asrc,string fileName) {
			Configuration conf = Configuration.getInstance();
			string filePath = applPath+conf.filePathPrefix+"/"+fileName;
			Dbg.msg(Dbg.Grp.Startup, 3, "loading audio clip:" + filePath);			

			WWW www = new WWW(filePath);
			yield return www;
			if(www.error != null) {
				Dbg.trc(Dbg.Grp.Sound, 3, www.error);
			}
			
			asrc.clip = www.GetAudioClip(false, false);
			if(asrc.clip == null) {
				Dbg.trc(Dbg.Grp.Sound, 3, "error loading audio clip = null");
			} else {
				asrc.clip.name = fileName;
				Dbg.trc(Dbg.Grp.Sound, 3, "loaded audio clip: " + asrc.clip.ToString()+":" + asrc.clip.name);
			}
		}
		
		public void rainSoundPlay(int type) {
			Dbg.trc(Dbg.Grp.Rain, 3, asrcCurrent.ToString()+":"+asrcCurrent.clip.name);

			if(asrcCurrent.isPlaying)
				asrcCurrent.Stop();

			switch (type) {

				case 1: asrcCurrent = asrcRainLight; break;
				case 2: asrcCurrent = asrcRainHeavy; break;
				case 3: asrcCurrent = asrcRainThunderstorm; break;
					
			}
			
			if(!asrcCurrent.isPlaying && asrcCurrent.clip.isReadyToPlay) {
				asrcCurrent.loop = true;
				asrcCurrent.volume = 0;
				asrcCurrent.Play();
				fader.SendMessage("OnDoFade",(object)true);
			}
		}
		
		public void rainSoundStop() {
			Dbg.trc(Dbg.Grp.Rain, 3, asrcCurrent.ToString()+":"+asrcCurrent.clip.name);
			fader.SendMessage("OnDoFade",(object)false);			
		}
	}	
}

