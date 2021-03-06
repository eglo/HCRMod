using System;
using System.Collections;
using UnityEngine;

namespace Plugin.HCR {

	public abstract class SingletonEntity<T> : Entity {
		protected static SingletonEntity<T> instance = null;

		//*****************************************************************************************		
		protected override void setup(
			Transform parent = null, 
			Vector3 ?position = null, 
			Quaternion ?rotation = null, 
			Vector3 ?scale = null, 
			string tag = "", 
			int layer  = 0
		) {
			Dbg.trcCaller(Dbg.Grp.Init, 3, "Singleton setup" + this.GetType().ToString());
			if (instance != null)
				throw new ArgumentException("Singleton already exists");
			
			base.setup(parent, position, rotation, scale, tag, layer);
			instance = this;
		}
			

		//*****************************************************************************************	
		/// get singleton entity by instance
		public static SingletonEntity<T> getInstance() {
			if (instance != null)
				return (SingletonEntity<T>) instance;
			else
				throw new ArgumentException("Singleton not created");			
		}

		//*****************************************************************************************	
		/// get an entity child
		public new static U GetEntity<U>() where U : Entity {
			object obj = instance.go.GetComponent(typeof(U));
			if (obj != null) {
				return (U) obj;
			} else {
				throw new ArgumentException("Entity not found");
			}
		}
		//*****************************************************************************************
		/// get an entity sibling
		public new static U GetEntityInParent<U>() where U : Entity {
			object obj = instance.gameObject.GetComponent(typeof(U));
			if (obj != null) {
				return (U) obj;
			} else {
				throw new ArgumentException("Entity not found");
			}
		}
	}

	//*****************************************************************************************

	public abstract class Entity : MonoBehaviour {
		protected GameObject go = new GameObject("iNiTiAl");
				
		//*****************************************************************************************				
		protected virtual void setup(
			Transform parent = null, 
			Vector3 ?position = null, 
			Quaternion ?rotation = null, 
			Vector3 ?scale = null, 
			string tag = "", 
			int layer  = 0 
		) {
			//Dbg.trcCaller(Dbg.Grp.Init, 3, "ExtMono setup" + this.GetType().ToString());
				
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
			//UnityEngine.Object.DontDestroyOnLoad(this.transform.gameObject);	

			if(parent != null) {
				this.transform.parent = parent;
				this.go.name = "ObjInst:" + this.GetType().ToString() + ":" + this.go.GetInstanceID().ToString("X8");
				//Dbg.trc(Dbg.Grp.Init, 3, "ExtMono setup done: " + this.go.name + " parent is:" + this.gameObject.name);
			} else {
				this.go.name = "ObjInst:" + this.GetType().ToString() + ":" + this.go.GetInstanceID().ToString("X8"); 
				//Dbg.trc(Dbg.Grp.Init, 3, "ExtMono setup done: " + this.go.name + " parent is: null");
			}
		}

		//*****************************************************************************************		
		protected virtual T AddEntity<T>(Transform parent)  where T : Entity {
			
			Dbg.trcCaller(Dbg.Grp.Init, 4,"Start AddEntity: "+typeof(T).FullName);
			
			T comp = go.AddComponent<T>();
			comp.setup(parent);
			
			Dbg.trcCaller(Dbg.Grp.Init, 5,"Done AddEntity: "+ comp.go.name + " parent is:" + comp.gameObject.name);			
			return comp;
		}

		//*****************************************************************************************		
		/// get an entity child

		public T GetEntity<T>() where T : Entity {
			object obj = go.GetComponent(typeof(T));
			if (obj != null) {
				Dbg.trcCaller(Dbg.Grp.All, 5,"GetEntity: "+typeof(T).FullName);
				return (T) obj;
			} else {
				throw new ArgumentException("Entity not found");
			}
		}

		//*****************************************************************************************		
		/// get an entity sibling

		public U GetEntityInParent<U>() where U : Entity {
			object obj = gameObject.GetComponent(typeof(U));
			if (obj != null) {
				return (U) obj;
			} else {
				throw new ArgumentException("Entity not found");
			}
		}
		
		//*****************************************************************************************		
		public abstract void Awake();
	}
}

