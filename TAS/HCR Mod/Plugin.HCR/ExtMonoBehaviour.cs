using System;
using System.Collections;
using UnityEngine;

namespace Plugin.HCR {

	public abstract class SingletonMonoBehaviour : ExtMonoBehaviour {
		protected static SingletonMonoBehaviour instance = null;

		public override void Setup(
			Transform parent = null, 
			Vector3 ?position = null, 
			Quaternion ?rotation = null, 
			Vector3 ?scale = null, 
			string tag = "", 
			int layer  = 0
		) {
			Dbg.trcCaller(Dbg.Grp.Init, 1, "Singleton setup" + this.GetType().ToString());
			base.Setup(parent, position, rotation, scale, tag, layer);
			instance = this;
			Dbg.trcCaller(Dbg.Grp.Init, 3, "Singleton setup done: " + this.GetType().ToString()); 				
		}
			
		public static SingletonMonoBehaviour getInstance() {
			Dbg.trcCaller(Dbg.Grp.Init, 3, "getInstance"); 				
			if(instance != null) {
				return instance;
			} else {
				throw new ArgumentException("Singleton not created");
			}
		}
	}


		///////////////////////////////////////////////////////////////////////////////////////////


	public abstract class ExtMonoBehaviour : MonoBehaviour {

		protected GameObject go = new GameObject();
		
		protected virtual T AddGameComponent<T>()  where T : ExtMonoBehaviour {
			T comp = go.AddComponent<T>();
			comp.transform.parent = this.transform;
			Dbg.trc(Dbg.Grp.Init, 3, typeof(T).FullName);
			return comp;
		}
		
		protected virtual T GetGameComponent<T>() where T : ExtMonoBehaviour {
			T comp = go.GetComponent<T>();
			Dbg.trc(Dbg.Grp.Init, 3, typeof(T).FullName);
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
			Dbg.trcCaller(Dbg.Grp.Init, 1, "Extmono setup: " + this.GetType().ToString());
	
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
		
			//this.go.active = true;
			//this.transform.gameObject.SetActive(true);

			//UnityEngine.Object.DontDestroyOnLoad(this.transform.gameObject);	//TODO: check if needed			
			if(parent != null) {
				this.transform.parent = parent.transform;
				this.go.name = "ObjInstance:" + this.GetType().ToString() + ":" + this.go.GetInstanceID().ToString("X8");	//this.GetType().ToString() + ":" + 
				Dbg.trc(Dbg.Grp.Init, 1, "ExtMono setup done: " + this.go.name + " parent is:" + this.transform.parent.gameObject.name);
			} else {
				this.go.name = "ObjInstance:" + this.GetType().ToString() + ":" + this.go.GetInstanceID().ToString("X8");	//this.GetType().ToString() + ":" + 
				//Dbg.trc(Dbg.Grp.Init, 1, "ExtMono setup done: " + this.gameObject.name + " parent is: " + this.transform.parent.ToString());
				Dbg.trc(Dbg.Grp.Init, 1, "ExtMono setup done: " + this.go.name + " parent is: null");
			}
		}

		public abstract void Awake();
	}
}

