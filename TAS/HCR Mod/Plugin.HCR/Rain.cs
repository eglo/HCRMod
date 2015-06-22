//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;
using Timber_and_Stone;

namespace Plugin.HCR {

	public class Rain : MonoBehaviour {
		
		public GameObject blob;
		public static List<GameObject> rainDropsOnMap = new List<GameObject>();
		public LineRenderer lineRenderer;
		public Vector3 location;
		
		private static Rain instance = new Rain();			
		public static Rain getInstance() {
			return instance; 
		}

		public Rain() {
			blob = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			blob.transform.localScale = new Vector3(0.2f,0.4f,0.2f);
//			blob.AddComponent<Rigidbody>();
//			blob.rigidbody.velocity = Vector3.down * .5f;
//			blob.AddComponent(typeof(ConstantForce));
//			blob.constantForce.relativeForce = Vector3.down * .5f;
//			blob.constantForce.enabled = true;
			//Physics.IgnoreCollision(blob.GetComponent<Collider>(), GetComponent<Collider>());
			//			lineRenderer = blob.AddComponent<LineRenderer>();
			//			lineRenderer.useWorldSpace = false;
			//			lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
			//			lineRenderer.SetColors(Color.blue, Color.green);
			blob.renderer.material = AManager<ChunkManager>.getInstance().materials[1];
			//location = new Vector3(UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f));
			//Instantiate(blob, base.transform.position+location, Quaternion.identity);
//			Display.printTrace(5,"new rainblob");
//			Display.printTrace(5,"Render active:"+blob.renderer.enabled.ToString());
//			Display.printTrace(5,"GameObj active:"+blob.active.ToString());
			blob.SetActiveRecursively(true);
		} 
		
		public void addRainDrop(Vector3 _location) {
			location = _location;
			//blob = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			//blob.transform.localScale = new Vector3(0.2f,0.4f,0.2f);
			//blob.AddComponent<Rigidbody>();
			//blob.rigidbody.velocity = new Vector3(UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f));
			//blob.AddComponent(typeof(ConstantForce));
			//blob.constantForce.relativeForce = Vector3.down * 5f;
			//blob.constantForce.enabled = true;
			//			lineRenderer = blob.AddComponent<LineRenderer>();
			//			lineRenderer.useWorldSpace = false;
			//			lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
			//			lineRenderer.SetColors(Color.blue, Color.green);
			//blob.renderer.material = AManager<ChunkManager>.getInstance().materials[1];
			//location = new Vector3(UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f));
			//Instantiate(blob, base.transform.position+location, Quaternion.identity);
			GameObject rainDrop = Instantiate(blob, location, Quaternion.identity) as GameObject;
			rainDropsOnMap.Add(rainDrop);
			//			Display.printTrace(5,"new rainblob");
			//			Display.printTrace(5,"Render active:"+blob.renderer.enabled.ToString());
			//			Display.printTrace(5,"GameObj active:"+blob.active.ToString());
			blob.SetActiveRecursively(true);
		} 

		public void Start() {
//			blob = GameObject.CreatePrimitive(PrimitiveType.Cube);
//			blob.AddComponent<Rigidbody>();
////			lineRenderer = blob.AddComponent<LineRenderer>();
////			lineRenderer.useWorldSpace = false;
////			lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
////			lineRenderer.SetColors(Color.blue, Color.green);
//			blob.renderer.material = AManager<ChunkManager>.getInstance().materials[1];
//			Display.printTrace(3,blob.renderer.enabled.ToString());
//			blob.rigidbody.velocity = new Vector3(UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f));
//			location = new Vector3(UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f));
//			Instantiate(blob, base.transform.position+location, Quaternion.identity);
//			blob.rigidbody.velocity = new Vector3(UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f));
//			location = new Vector3(UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f));
//			Instantiate(blob, base.transform.position+location, Quaternion.identity);
//			blob.rigidbody.velocity = new Vector3(UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f));
//			location = new Vector3(UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f));
//			Instantiate(blob, base.transform.position+location, Quaternion.identity);
			//constantForce.relativeForce = new Vector3(0, -0.01f, 0);
			//Display.printTrace(3,"Render active:"+blob.renderer.enabled.ToString());
			//Display.printTrace(3,"GameObj active:"+blob.active.ToString());
		}
		
		public void Update() {
			//Display.beep();
			//Display.printTrace(5,"Render active:"+blob.renderer.enabled.ToString());
			Display.printTrace(5,"GameObj active:"+blob.active.ToString());
			foreach (GameObject rainDrop in rainDropsOnMap) {
				rainDrop.transform.Translate(new Vector3(0,-.2f,-.05f) * Time.deltaTime,Space.World);
				if(rainDrop.transform.position.y < (float)(location.y-10))
					rainDrop.transform.position = location;

			}
			//blob.rigidbody.velocity = new Vector3(0,UnityEngine.Random.Range(-0.5f,0),0);
		}

		public void OnCollisionStay() {
			Display.printTrace(5,blob.name.ToString()+" collided with another object");
		}

	}	
}

