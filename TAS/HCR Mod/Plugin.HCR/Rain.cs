using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timber_and_Stone;


namespace Plugin.HCR {

	public class Rain : SingletonMonoBehaviour {

		public static List<RainDrop> rainDropsOnMap = new List<RainDrop>();
		public bool isRainOnMap = false;
		public float timeToRemove = 0.0f;
	
		public class RainDrop {
			
			public GameObject blob;
			public Vector3 location;
			public Vector3 minHeight;
			
			public RainDrop(Vector3 _location, Vector3 _minHeight) {
				location = _location;
				minHeight = _minHeight;
				blob = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				blob.transform.localScale = new Vector3(0.15f,0.3f,0.15f);
				blob.renderer.material = AManager<ChunkManager>.getInstance().materials[1];
			}		
		}

		public void startRain() {
			Dbg.trc(Dbg.Grp.Rain, 3);

			//TODO: shouldnt really happen at this point
			if(isRainOnMap) {
				return;
			}
			RainSound rs = GetGameComponent<RainSound>();
			rs.rainSoundPlay();
			//stay on map for about 5-10 mins, this doesn't care about game speed settings ..(?)
			timeToRemove = Time.time + UnityEngine.Random.Range(300.0f, 600.0f);
			
			isRainOnMap = true;
		} 

		public void addRainDrop(Vector3 location, Vector3 minHeight) {
			RainDrop rainDrop = new RainDrop(location,minHeight);
			rainDrop.blob = Instantiate(rainDrop.blob, location, Quaternion.identity) as GameObject;
			rainDropsOnMap.Add(rainDrop);
			rainDrop.blob.SetActiveRecursively(true);	//TODO: is this needed?
			Dbg.trc(Dbg.Grp.Rain, 2);			
		} 

		public void removeRain() {
			Dbg.trc(Dbg.Grp.Rain, 3);

			foreach(RainDrop rainDrop in rainDropsOnMap) {
				UnityEngine.Object.Destroy(rainDrop.blob); 
			}
			rainDropsOnMap.Clear();
			RainSound rs = GetGameComponent<RainSound>();;
			rs.rainSoundStop();

			isRainOnMap = false;
			UI.print("The rain has stopped");
		} 
		
		public override void Awake() {
			Dbg.trc(Dbg.Grp.Init, 3);
			try {
				AddGameComponent<RainSound>(this.transform);
				AddGameComponent<Lightning>(this.transform);
			} catch(Exception e) { 
				Dbg.dumpExc(e);
			}
		}
		

		public void Start() {
			Dbg.trc(Dbg.Grp.Startup, 3);
			
		}
		
		public void Update() {
			//Dbg.trc(Dbg.Grp.Rain,1,"Rain update");
			float speed = 2.0f;
			foreach(RainDrop rainDrop in rainDropsOnMap) {
				rainDrop.blob.transform.Translate(new Vector3(0,-1f,-.2f) * Time.deltaTime * speed, Space.World);
				if(rainDrop.blob.transform.position.y < rainDrop.minHeight.y) {
					rainDrop.blob.transform.position = rainDrop.location;
				}
			}
		}
	}	
}

