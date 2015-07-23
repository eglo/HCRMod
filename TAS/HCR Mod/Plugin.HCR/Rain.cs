using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timber_and_Stone;


namespace Plugin.HCR {

	public class Rain : SingletonEntity<Rain> {

		public static List<RainDrop> rainDropsOnMap = new List<RainDrop>();
		public bool isRainOnMap = false;
		public float timeToRemove = 0.0f;
	
		//*****************************************************************************************
		public class RainDrop {
			
			public GameObject go;
			public Vector3 location;
			public Vector3 minHeight;
			
			public RainDrop(Vector3 _location, Vector3 _minHeight) {
				location = _location;
				minHeight = _minHeight;
				go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				go.transform.localScale = new Vector3(0.1f,0.2f,0.1f);
				go.renderer.material = AManager<ChunkManager>.getInstance().materials[1];
			}		
		}

		//*****************************************************************************************
		public override void Awake() {
			Dbg.trc(Dbg.Grp.Init, 5);
		}
		
		public void Start() {
			Dbg.trc(Dbg.Grp.Startup, 5);
			
			try {
				AddEntity<RainSound>(this.transform);
				AddEntity<Lightning>(this.transform);
			} catch(Exception e) { 
				Dbg.dumpExc(e);
			}
		}
		
		public void Update() {
			//Dbg.trc(Dbg.Grp.Rain,3,"Rain update");
			float speed = 2.0f;
			foreach(RainDrop rainDrop in rainDropsOnMap) {
				rainDrop.go.transform.Translate(new Vector3(0,-1f,-.2f) * Time.deltaTime * speed, Space.World);
				if(rainDrop.go.transform.position.y < rainDrop.minHeight.y) {
					rainDrop.go.transform.position = rainDrop.location;
				}
			}
		}

		//*****************************************************************************************
		public void startRain(int type) {
			Dbg.trc(Dbg.Grp.Rain, 5);

			//TODO: check: shouldnt really happen at this point, but it does...?
			if(isRainOnMap) {
				return;
			}
			isRainOnMap = true;

			RainSound rs = GetEntity<RainSound>();
			rs.rainSoundPlay(type);
			//stay on map for about 5-10 mins, this doesn't care about game speed settings ..(?)
			timeToRemove = Time.time + UnityEngine.Random.Range(300.0f, 600.0f);
			
			isRainOnMap = true;
		} 

		//*****************************************************************************************
		public void addRainDrop(Vector3 location, Vector3 minHeight) {
			RainDrop rainDrop = new RainDrop(location,minHeight);
			rainDrop.go = Instantiate(rainDrop.go, location, Quaternion.identity) as GameObject;
			//rainDrop.go.transform.tag = "RainDrop"+rainDropsOnMap.Count.ToString();
			rainDropsOnMap.Add(rainDrop);
			rainDrop.go.SetActiveRecursively(true);	//TODO: check: is this needed?
			Dbg.trc(Dbg.Grp.Rain, 3);			
		} 

		//*****************************************************************************************
		public void removeRain() {
			Dbg.trc(Dbg.Grp.Rain, 5);

			foreach(RainDrop rainDrop in rainDropsOnMap) {
				UnityEngine.Object.Destroy(rainDrop.go); 
			}
			rainDropsOnMap.Clear();
			RainSound rs = GetEntity<RainSound>();;
			rs.rainSoundStop();

			isRainOnMap = false;
			//UI.print("The rain has stopped");
		} 
		
	}	
}

