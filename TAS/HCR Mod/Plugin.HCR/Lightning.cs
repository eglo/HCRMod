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
		public AudioSource asrc;
		public LineRenderer rend;
		public new Light light;
		public Vector3 pos;
		public static List<Bolt> boltSegments = new List<Bolt>();
		
		public class Bolt  {
			
			public GameObject go;
			public Vector3 location;
			public Vector3 minHeight;
			
			public Bolt(Vector3 _location, Vector3 _minHeight) {
				location = _location;
				minHeight = _minHeight;
				go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
				go.transform.localScale = new Vector3(1.0f,10.0f,1.0f);
				go.renderer.material = AManager<ChunkManager>.getInstance().materials[30];
			}		
		}
		
		//*****************************************************************************************		

		public override void Awake() {
			Dbg.trc(Dbg.Grp.Init, 5);
			
			light = go.AddComponent<Light>();
			light.color = Color.white;
			light.type = LightType.Directional;
			light.intensity = 0;
			light.range = 0;

			rend = go.AddComponent<LineRenderer>();
			rend.useWorldSpace = true;
			rend.material = new Material(Shader.Find("Particles/Additive"));
			rend.SetColors(Color.white, Color.white);
			rend.SetWidth(0.2F, 0.2F);
			rend.SetVertexCount(20);

			pos = new Vector3(0,15,0);
			go.transform.position = pos;

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
				Rain rain = Rain.getInstance() as Rain;
				Dbg.trc(Dbg.Grp.All, 5,"rain= "+rain.ToString());
				
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

				if (intensity < 0.006f) {
					rend.enabled = false;
					//pos = new Vector3(UnityEngine.Random.Range(0,20), 15, UnityEngine.Random.Range(0,20));
					pos = new Vector3(0, 15, 0);
					go.transform.position = pos;
				} else if (intensity >= 0.006f) {
					int i = 0;
					while (i < 20) {
						Vector3 lpos = pos;
						lpos.x += UnityEngine.Random.value*2-1;
						lpos.y -= i+UnityEngine.Random.value*2-1;
						lpos.z += UnityEngine.Random.value*2-1;
						rend.SetPosition(i, lpos);
						i++;
					}
					rend.SetWidth(intensity*20,intensity*20);
					rend.enabled = true;
				}
									
				if (intensity < 0.003f) {
					intensity *= 0f;
				} else if (intensity < 0.006f)
					intensity *= 10f;
				else if (intensity < 0.008f)
					intensity *= 20f;
				else if (intensity < 0.01)
					intensity *= 100f;	
				else 
					intensity *= 200f;	
				
				go.light.intensity = intensity;
				go.light.range = intensity;	//does this even have an effect with directional light?
				
			} catch (Exception e) {
				Dbg.dumpExc(e);
			}
		}
	}
}		