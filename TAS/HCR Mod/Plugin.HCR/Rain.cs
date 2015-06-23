
using System;
using System.Collections.Generic;
using UnityEngine;
using Timber_and_Stone;

namespace Plugin.HCR {


	public class RainDrop {

		public GameObject blob;
		public Vector3 location;
		
		public RainDrop(Vector3 _location) {
			location = _location;
			blob = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			blob.transform.localScale = new Vector3(0.2f,0.4f,0.2f);
//			blob.AddComponent<Rigidbody>();
//			blob.rigidbody.velocity = Vector3.down * .5f;
//			blob.AddComponent(typeof(ConstantForce));
//			blob.constantForce.relativeForce = Vector3.down * .5f;
//			blob.constantForce.enabled = true;
//			LineRenderer lineRenderer;
//			lineRenderer = blob.AddComponent<LineRenderer>();
//			lineRenderer.useWorldSpace = false;
//			lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
//			lineRenderer.SetColors(Color.blue, Color.green);
			blob.renderer.material = AManager<ChunkManager>.getInstance().materials[1];
		}
		
	}

	public class Rain : MonoBehaviour {
		
		public static List<RainDrop> rainDropsOnMap = new List<RainDrop>();
		public bool isRainOnMap = false;
		
		private static Rain instance = new Rain();			
		public static Rain getInstance() {
			return instance; 
		}

		public Rain() {
		} 
		
		public void addRainDrop(Vector3 location) {
			RainDrop rainDrop = new RainDrop(location);
			rainDrop.blob = Instantiate(rainDrop.blob, location, Quaternion.identity) as GameObject;
			rainDropsOnMap.Add(rainDrop);
			isRainOnMap = true;
			//rainDrop.blob.SetActiveRecursively(true);
		} 

		public void removeRainDrops() {
			foreach (RainDrop rainDrop in rainDropsOnMap) {
				UnityEngine.Object.Destroy(rainDrop.blob); 
			}
			rainDropsOnMap.Clear();
			isRainOnMap = false;
		} 
		
		public void Start() {
			Display.printDebug(5,"Rain started");
		}
		
		public void Update() {
			foreach (RainDrop rainDrop in rainDropsOnMap) {
				rainDrop.blob.transform.Translate(new Vector3(0,-.5f,-.1f) * Time.deltaTime,Space.World);
				if(rainDrop.blob.transform.position.y < (float)(rainDrop.location.y-10))
					rainDrop.blob.transform.position = rainDrop.location;

			}
		}

	}	
}

