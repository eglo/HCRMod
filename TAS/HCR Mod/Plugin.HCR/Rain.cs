
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timber_and_Stone;


namespace Plugin.HCR {
	
	public class RainSound : MonoBehaviour {
		private static string filePath = "file:///"+Application.dataPath+"/StreamingAssets/rain.ogg";
		private static WWW www = new WWW(filePath);
		public bool isActive = false;
		public static GameObject go = new GameObject();
		public static AudioSource ass;
		
		private static RainSound instance = new RainSound();			
		public static RainSound getInstance() {
			return instance; 
		}

		public void Start() {
			Dbg.trc(Dbg.Grp.Startup,3,"RainSound started");
			Dbg.msg(Dbg.Grp.Rain,3,"Using rain sound file:"+filePath);
			ass = (AudioSource) go.AddComponent(typeof(AudioSource));
			StartCoroutine(rainSoundLoad());
		}

		IEnumerator rainSoundLoad() {
			yield return www;
			if(www.error != null)
				Dbg.trc(Dbg.Grp.Rain,3,www.error);

			ass.clip = www.GetAudioClip(false,false);
			if (ass.clip == null) {
				Dbg.trc(Dbg.Grp.Rain,3,"clip == null");
			} else {
				Dbg.trc(Dbg.Grp.Rain,3,"clip == "+ass.clip.ToString());
			}
			Dbg.trc(Dbg.Grp.Rain,3,"done");
//			if (!ass.isPlaying && ass.clip.isReadyToPlay) {
//				ass.volume = 1.0f;
//				ass.loop = true;
//				ass.Play();
//			}			
		}

		public void rainSoundPlay() {
			Dbg.trc(Dbg.Grp.Rain,3,ass.ToString());

			if (!ass.isPlaying && ass.clip.isReadyToPlay) {
				ass.volume = 1.0f;
				ass.loop = true;
				ass.Play();
			}			
		}

		public void rainSoundStop() {
			Dbg.trc(Dbg.Grp.Rain,3,ass.ToString());
			ass.Pause();
		}
		
	}

	public class RainDrop {

		public GameObject blob;
		public Vector3 location;
		public Vector3 minHeight;
		
		public RainDrop(Vector3 _location,Vector3 _minHeight) {
			location = _location;
			minHeight = _minHeight;
			blob = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			blob.transform.localScale = new Vector3(0.15f,0.3f,0.15f);
			blob.renderer.material = AManager<ChunkManager>.getInstance().materials[1];
		}		
	}

	public class Rain : MonoBehaviour {

		public static GameObject go = new GameObject();
		public static List<RainDrop> rainDropsOnMap = new List<RainDrop>();
		public bool isRainOnMap = false;
		public float timeToRemove = 0.0f;
	
		private static Rain instance = new Rain();			
		public static Rain getInstance() {
			return instance; 
		}
		
		public void startRain() {
			Dbg.trc(Dbg.Grp.Rain,3);

			//shouldnt happen
			if(isRainOnMap)
				return;

			RainSound rs = RainSound.getInstance();
			rs.rainSoundPlay();
			//stay on map for about 5-10 mins, this doesn't care about game speed settings ..(?)
			timeToRemove = Time.time+UnityEngine.Random.Range(300.0f,600.0f);
			
			isRainOnMap = true;
		} 

		public void addRainDrop(Vector3 location,Vector3 minHeight) {
			RainDrop rainDrop = new RainDrop(location,minHeight);
			rainDrop.blob = Instantiate(rainDrop.blob, location, Quaternion.identity) as GameObject;
			rainDropsOnMap.Add(rainDrop);
			rainDrop.blob.SetActiveRecursively(true);	//TODO: is this needed?
			Dbg.trc(Dbg.Grp.Rain,2);			
		} 

		public void removeRain() {
			Dbg.trc(Dbg.Grp.Rain,3);

			foreach (RainDrop rainDrop in rainDropsOnMap) {
				UnityEngine.Object.Destroy(rainDrop.blob); 
			}
			rainDropsOnMap.Clear();

			RainSound rs = RainSound.getInstance();
			rs.rainSoundStop();

			isRainOnMap = false;

			UI.print("The rain has stopped");
		} 
		
		public void Start() {
			gameObject.AddComponent(typeof(AudioSource));
			gameObject.transform.position = Weather.getInstance().worldSize3i/2;
			gameObject.AddComponent(typeof(RainSound));
			
			Dbg.trc(Dbg.Grp.Startup,3,"Rain started");
		}
		
		public void Update() {
			//Dbg.trc(Dbg.Grp.Rain,1,"Rain update");
			foreach (RainDrop rainDrop in rainDropsOnMap) {
				rainDrop.blob.transform.Translate(new Vector3(0,-1f,-.2f) * Time.deltaTime,Space.World);
				if(rainDrop.blob.transform.position.y < rainDrop.minHeight.y)
					rainDrop.blob.transform.position = rainDrop.location;
			}
		}
	}	
}

