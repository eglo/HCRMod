
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timber_and_Stone;

namespace Plugin.HCR {


	public class RainDrop {

		public GameObject blob;
		public Vector3 location;
		public Vector3 minHeight;
		
		public RainDrop(Vector3 _location,Vector3 _minHeight) {
			location = _location;
			minHeight = _minHeight;
			blob = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			//blob = GameObject.CreatePrimitive(PrimitiveType.Cube);
			blob.transform.localScale = new Vector3(0.2f,0.4f,0.2f);
			blob.renderer.material = AManager<ChunkManager>.getInstance().materials[1];
		}
		
	}

	public class Rain : MonoBehaviour {
		
		public static List<RainDrop> rainDropsOnMap = new List<RainDrop>();
		public bool isRainOnMap = false;
		public float timeToRemove = 0.0f;
		
		private static Rain instance = new Rain();			
		public static Rain getInstance() {
			return instance; 
		}

		public Rain() {
		} 
		
		public void addRainDrop(Vector3 location,Vector3 minHeight) {
			RainDrop rainDrop = new RainDrop(location,minHeight);
			rainDrop.blob = Instantiate(rainDrop.blob, location, Quaternion.identity) as GameObject;
			rainDropsOnMap.Add(rainDrop);
			if (!isRainOnMap) {
				//stay on map for about 5-10 mins, this doesn't care about game speed settings ..(?)
				timeToRemove = Time.time+UnityEngine.Random.Range(300.0f,600.0f);
				isRainOnMap = true;
			}
			rainDrop.blob.SetActiveRecursively(true);	//needed?
			Dbg.trc(Dbg.Grp.Rain,2);
			
		} 

		public void removeRainDrops() {
			Dbg.trc(Dbg.Grp.Rain,3);
			foreach (RainDrop rainDrop in rainDropsOnMap) {
				UnityEngine.Object.Destroy(rainDrop.blob); 
			}
			rainDropsOnMap.Clear();
			isRainOnMap = false;

			UI.print("The rain has stopped");
		} 
		
		public void Start() {
			Dbg.msg(Dbg.Grp.Startup,3,"Rain started");
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

