using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timber_and_Stone;

namespace Plugin.HCR {
	
	
	public class Lightning : SingletonMonoBehaviour {

		private const int samples = 256;
		private int fMin = 20;
		private int fMax = 8000;
		private int bLow;
		private int bHigh;
		private float[] freqData = new float[samples];
		private AudioSource asrc;		
		private GameObject lightGameObject = new GameObject("Lightning light");
		
		public override void Awake() {
			Dbg.trc(Dbg.Grp.Init, 3);
			
			lightGameObject.AddComponent<Light>();
			//lightGameObject.light.color = Color.white;
			lightGameObject.light.color = new Color(0.80f,0.80f,1.0f);	//tried to get a blueish tint, but cant really see any of it it haha
			lightGameObject.light.type = LightType.Directional;
			lightGameObject.light.intensity = 0;
			lightGameObject.light.range = 0;
			lightGameObject.transform.parent = go.transform;
			lightGameObject.transform.position = new Vector3(5, 10, 5);			

			int fSmplRate = AudioSettings.outputSampleRate/2;
			bLow = (fMin/(fSmplRate/samples));
			bHigh = (fMax/(fSmplRate/samples));
		}
		
		public void Start() {
			Dbg.trc(Dbg.Grp.Startup, 3);

			RainSound rs = gameObject.GetComponent<RainSound>();
			//the clip isnt valid at this point, do I need yet another coroutine? .. :/
			//asrc = rs.asrcCurrent;
			//Dbg.trc(Dbg.Grp.Startup|Dbg.Grp.Light, 3,"Audiosource linked: "+asrc.gameObject.name+":"+asrc.name);			
			//Dbg.trc(Dbg.Grp.Startup|Dbg.Grp.Sound, 3, "clip: " + asrc.clip.ToString()+":" + asrc.clip.name);
		}

		public void Update() {
			try {			
				RainSound rs = gameObject.GetComponent<RainSound>();
				asrc = rs.asrcCurrent;
				
				if (!asrc || !asrc.isPlaying)
					return;

				Dbg.trc(Dbg.Grp.Light, 3);
				Dbg.trc(Dbg.Grp.Light, 3,"Audiosource linked: "+asrc.gameObject.name+":"+asrc.name);			
				Dbg.trc(Dbg.Grp.Sound, 3, "clip: " + asrc.clip.ToString()+":" + asrc.clip.name);
				
				asrc.GetSpectrumData(freqData,0,FFTWindow.BlackmanHarris);
				float vol = 0.0f;
				// average the volumes of frequencies
				for (int i=bLow; i<=bHigh; i++){
					vol += freqData[i];
				}
				vol /= (bHigh - bLow + 1);
				Dbg.trc(Dbg.Grp.Light, 3, "vol = "+ vol.ToString());

				//values after fft are very small, not sure why
				//cut off values just from trial and error for now 
				//needs a better selection of frequencies, 20-8000 is probably not optimal at all
				vol *= asrc.volume;
				float intensity = vol;
				if (intensity < 0.003f)
					intensity *= 0f;	
				else if (intensity < 0.006f)
					intensity *= 10f;
				else if (intensity < 0.008f)
					intensity *= 20f;
				else if (intensity < 0.01)
					intensity *= 100f;	
				else 
					intensity *= 200f;	
				
				lightGameObject.light.intensity = intensity;
				lightGameObject.light.range = intensity;	//hmm...?
				
			} catch (Exception e) {
				Dbg.dumpExc(e);
			}
		}
	}
}		