using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timber_and_Stone;

namespace Plugin.HCR {
	
	
	public class Lightning : SingletonEntity<Lightning> {

		private const int samples = 256;
		private int fMin = 20;
		private int fMax = 8000;
		private int bLow;
		private int bHigh;
		private float[] freqData = new float[samples];
		private AudioSource asrc;		
		private GameObject lightGameObject = new GameObject("Lightning light");
		private Vector3 pos;

		public class Bolt  {
			
			public GameObject bolt;
			public Vector3 location;
			public Vector3 minHeight;
			
			public Bolt(Vector3 _location, Vector3 _minHeight) {
				location = _location;
				minHeight = _minHeight;
				bolt = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
				bolt.transform.localScale = new Vector3(1.0f,10.0f,1.0f);
				bolt.renderer.material = AManager<ChunkManager>.getInstance().materials[3];
			}		
		}
		
		//*****************************************************************************************		

		public override void Awake() {
			Dbg.trc(Dbg.Grp.Init, 5);
			
			lightGameObject.AddComponent<Light>();
			lightGameObject.light.color = Color.white;
			lightGameObject.light.type = LightType.Directional;
			lightGameObject.light.intensity = 0;
			lightGameObject.light.range = 0;
			lightGameObject.transform.parent = go.transform;
			pos = new Vector3(UnityEngine.Random.Range(0,20), 20, UnityEngine.Random.Range(0,20));
			lightGameObject.transform.position = pos;

			int fSmplRate = AudioSettings.outputSampleRate/2;
			bLow = (fMin/(fSmplRate/samples));
			bHigh = (fMax/(fSmplRate/samples));
		}
		
		public void Start() {
			Dbg.trc(Dbg.Grp.Startup, 5);

			RainSound rs = gameObject.GetComponent<RainSound>();
			//the clip isnt valid at this point, do I need yet another coroutine? .. :/
			//asrc = rs.asrcCurrent;
			//Dbg.trc(Dbg.Grp.Startup|Dbg.Grp.Light, 5,"Audiosource linked: "+asrc.gameObject.name+":"+asrc.name);			
			//Dbg.trc(Dbg.Grp.Startup|Dbg.Grp.Sound, 5, "clip: " + asrc.clip.ToString()+":" + asrc.clip.name);
		}

		public void Update() {
			try {			
				RainSound rs = gameObject.GetComponent<RainSound>();
				asrc = rs.asrcCurrent;
				
				if (!asrc || !asrc.isPlaying)
					return;

				Dbg.trc(Dbg.Grp.Light, 3);
				Dbg.trc(Dbg.Grp.Light, 2,"Audiosource linked: "+asrc.gameObject.name+":"+asrc.name);			
				Dbg.trc(Dbg.Grp.Sound, 2, "clip: " + asrc.clip.ToString()+":" + asrc.clip.name);
				
				asrc.GetSpectrumData(freqData,0,FFTWindow.BlackmanHarris);
				float vol = 0.0f;
				// average the volumes of frequencies
				for (int i=bLow; i<=bHigh; i++){
					vol += freqData[i];
				}
				vol /= (bHigh - bLow + 1);
				Dbg.trc(Dbg.Grp.Light, 2, "vol = "+ vol.ToString());

				//cut off values just from trial and error for now 
				//needs a better selection of frequencies, 20-8000 is probably not optimal at all
				vol *= asrc.volume;
				float intensity = vol;
				if (intensity < 0.003f) {
					pos = new Vector3(UnityEngine.Random.Range(0,20), 20, UnityEngine.Random.Range(0,20));
					lightGameObject.transform.position = pos;					
					intensity *= 0f;	
				} else if (intensity < 0.006f)
					intensity *= 10f;
				else if (intensity < 0.008f)
					intensity *= 20f;
				else if (intensity < 0.01)
					intensity *= 100f;	
				else 
					intensity *= 200f;	
				
				lightGameObject.light.intensity = intensity;
				lightGameObject.light.range = intensity;	//does this even have an effect with directional light?
				
			} catch (Exception e) {
				Dbg.dumpExc(e);
			}
		}
	}
}		