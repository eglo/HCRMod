using System;
using System.Collections;
using UnityEngine;

namespace Plugin.HCR {


	public abstract class ExtMonoBehaviour<T> : MonoBehaviour {
		private static ExtMonoBehaviour<T> instance = null;
		protected GameObject go = null;
		
		protected ExtMonoBehaviour(GameObject parent, bool singleton = false, string tag = "", int layer  = 0) {
				if (singleton) {
					if (instance == null) {
						instance = this;
					} else {
						throw new ArgumentException("Singleton already instantiated");
					}
				}
				go = new GameObject();
				go.AddComponent(typeof(T));
				go.name = "ObjInstance:"+this.GetType().ToString()+":"+GetInstanceID().ToString();
				go.tag = tag;
				go.layer = layer;
				go.transform.parent = parent.transform;
				UnityEngine.Object.DontDestroyOnLoad(go);	//TODO: check if needed			
		}			

		protected static ExtMonoBehaviour<T> getInstance() {
			if (instance == null) {
				throw new ArgumentException("Singleton not instantiated");
			} else {
				return instance;
			}
		}		

		protected Component AddComponent<T2>() {
			return go.AddComponent(typeof(T2)); 
		}

		public virtual void Awake() {
			Dbg.trc(Dbg.Grp.Init,1,this.GetType().ToString()+":Awake has no base class override, you sure want this?");
		}

		public virtual void Start() {
			Dbg.trc(Dbg.Grp.Init,1,this.GetType().ToString()+":Start has no base class override, you sure want this?");
		}

		public virtual void Update() {
			Dbg.trc(Dbg.Grp.Init,1,this.GetType().ToString()+":Update has no base class override, you sure want this?");
		}
		

	}
}

