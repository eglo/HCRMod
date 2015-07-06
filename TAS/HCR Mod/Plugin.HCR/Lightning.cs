using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timber_and_Stone;

namespace Plugin.HCR {
	
	
	public class Lightning : SingletonMonoBehaviour {

		private const int samples = 256;
		private int fMin = 20;
		private int fMax = 400;
		private int bLow;
		private int bHigh;
		private float[] freqData = new float[samples];
		private AudioSource asrc;		
		private GameObject lightGameObject = new GameObject("Lightning light");
		
		public override void Awake() {
			Setup();
		}
		
		public void Start() {
			Dbg.trc(Dbg.Grp.Startup, 3, "Lightning started");

			lightGameObject.AddComponent<Light>();
			lightGameObject.light.color = Color.white;
			lightGameObject.transform.position = new Vector3(0, 5, 0);

			asrc = transform.parent.GetComponent<AudioSource>();
			Dbg.trc(Dbg.Grp.Startup|Dbg.Grp.Light,3," asrc = "+asrc.name);
			int fSmplRate = AudioSettings.outputSampleRate/2;
			bLow = (fSmplRate/samples*fMin);
			bHigh = (fSmplRate/samples / fMax);

		}

		public void Update() {
			try {			
				Dbg.trc(Dbg.Grp.Light, 3);

				asrc.GetSpectrumData(freqData,0,FFTWindow.BlackmanHarris);
				float vol = 0.0f;
				// average the volumes of frequencies
				for (int i=bLow; i<=bHigh; i++){
					vol += freqData[i];
				}
				vol /= (bHigh - bLow + 1);
				Dbg.trc(Dbg.Grp.Light, 3, "vol = "+ vol.ToString());
				
				light.intensity = 3.0f * vol;
			} catch (Exception e) {
				Dbg.dumpExc(e);
			}
		}
	}
}		