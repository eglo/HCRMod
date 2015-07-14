using System;
using System.Collections;
using UnityEngine;

namespace Plugin.HCR {

	public abstract class SingletonEntity<T> : Entity {
		protected static SingletonEntity<T> instance = null;

		public override void Setup(
			Transform parent = null, 
			Vector3 ?position = null, 
			Quaternion ?rotation = null, 
			Vector3 ?scale = null, 
			string tag = "", 
			int layer  = 0
		) {
			Dbg.trcCaller(Dbg.Grp.Init, 1, "Singleton setup" + this.GetType().ToString());
//TODO this is bull...
			if (instance != null)
				throw new ArgumentException("Singleton already created");
			
			base.Setup(parent, position, rotation, scale, tag, layer);
			instance = this;
		}
			
		public static U GetEntity<U>() where U : Entity {
			object obj = instance.go.GetComponent(typeof(U));
			if (obj != null) {
				return (U) obj;
			} else {
				throw new ArgumentException("Entity not found");
			}
		}
		public static U GetEntityInParent<U>() where U : Entity {
			object obj = instance.gameObject.GetComponent(typeof(U));
			if (obj != null) {
				return (U) obj;
			} else {
				throw new ArgumentException("Entity not found");
			}
		}
		public static U FindEntity<U>() where U : Entity {
			object obj = GameObject.FindObjectOfType(typeof(U));
			if (obj != null) {
				return (U) obj;
			} else {
				throw new ArgumentException("Gameobject not found");
			}
		}
	}

	///////////////////////////////////////////////////////////////////////////////////////////


	public abstract class Entity : MonoBehaviour {
		protected GameObject go = new GameObject("iNiTiAl");
		
		protected virtual T AddGameComponent<T>(Transform parent)  where T : Entity {

			Dbg.trcCaller(Dbg.Grp.Init, 2 ,"Start AddGameComponent: "+typeof(T).FullName);

			T comp = go.AddComponent<T>();
			comp.Setup(parent);
		
			Dbg.trcCaller(Dbg.Grp.Init, 3,"Done AddGameComponent: "+ comp.go.name + " parent is:" + comp.gameObject.name);			
			return comp;
		}
		
		public T GetGameComponent<T>() where T : Entity {
			T comp = go.GetComponent<T>();
			Dbg.trcCaller(Dbg.Grp.Init, 3,"GetGameComponent: "+typeof(T).FullName);
			return comp;
		}
		
		public virtual void Setup(
			Transform parent = null, 
			Vector3 ?position = null, 
			Quaternion ?rotation = null, 
			Vector3 ?scale = null, 
			string tag = "", 
			int layer  = 0 
		) {
			//Dbg.trcCaller(Dbg.Grp.Init, 1, "ExtMono setup" + this.GetType().ToString());
				
			if(position == null) {
				this.transform.localPosition = Vector3.one;
			} else {
				this.transform.localPosition = (Vector3)position;
			}
			if(rotation == null) {
				this.transform.localRotation = Quaternion.identity;
			} else {
				this.transform.localRotation = (Quaternion)rotation;
			}
			if(scale == null) {
				this.transform.localScale = Vector3.one;
			} else {
				this.transform.localScale = (Vector3)scale;
			}
			this.go.tag = tag;
			this.go.layer = layer;
		
			//this.transform.gameObject.SetActive(true);
			this.go.active = true;
			//UnityEngine.Object.DontDestroyOnLoad(this.transform.gameObject);	//TODO: check if needed			

			if(parent != null) {
//TODO:check this
				this.transform.parent = parent;
				this.go.name = "ObjInst:" + this.GetType().ToString() + ":" + this.go.GetInstanceID().ToString("X8");
				//Dbg.trc(Dbg.Grp.Init, 1, "ExtMono setup done: " + this.go.name + " parent is:" + this.gameObject.name);
			} else {
				this.go.name = "ObjInst:" + this.GetType().ToString() + ":" + this.go.GetInstanceID().ToString("X8"); 
				//Dbg.trc(Dbg.Grp.Init, 1, "ExtMono setup done: " + this.go.name + " parent is: null");
			}
		}

		public GameObject getGameObject() {
			return go;
		}

		public abstract void Awake();
	}
}

