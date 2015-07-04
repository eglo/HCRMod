using System;
using System.Collections;
using UnityEngine;

namespace Plugin.HCR {
	public abstract class MyManager<T> : MonoBehaviour where T : MyManager<T> {
		private static T instance;

		public static T getInstance() {
			return MyManager<T>.instance;
		}

		protected virtual void Awake() {
			if(MyManager<T>.instance == null) {
				MyManager<T>.instance = (this as T);
			} else if(MyManager<T>.instance != this as T) {
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}


	}

	public class SingletonMonoBehaviour<T> : ExtMonoBehaviour<T> where T : SingletonMonoBehaviour<T> {
		private static SingletonMonoBehaviour<T> instance;

		private SingletonMonoBehaviour() : base(null) {
		}

		protected SingletonMonoBehaviour(GameObject parent = null, string tag = "", int layer  = 0) 
			: base (parent, tag, layer) {
			Dbg.trc(Dbg.Grp.Init, 3, "single ctor");
			
		}

		public static T Create<U>(GameObject parent, string tag = "", int layer  = 0) where U : T, new() {
			Dbg.trc(Dbg.Grp.Init, 3, "Creating single");
			instance = new U();
			instance.go.tag = tag;
			instance.go.layer = layer;
			if(parent != null) {
				instance.go.transform.parent = parent.transform;
			}
			instance.go.name = "Singleton:" + instance.GetType().ToString() + ":" + instance.go.GetInstanceID().ToString("X8");
			instance.go.AddComponent(typeof(T));
			Dbg.trc(Dbg.Grp.Init, 3, "Created " + instance.go.name);
			return instance as T;
		}

		public static T getInstance() {
			Dbg.trc(Dbg.Grp.Init, 3, "Get singleton");
//			if(instance != null) {
//				return instance as T;
//			} else {
//				throw new ArgumentException("Singleton not created");
//			}
			return SingletonMonoBehaviour<T>.instance as T;
		}

#pragma warning disable 0465
		private void Finalize() {
			//oops
			Dbg.trc(Dbg.Grp.All, 10, "Black hawk down..");			
		}
		
	}

	public class ExtMonoBehaviour<T> : MonoBehaviour {
		protected GameObject go = null;
		
		protected ExtMonoBehaviour(GameObject parent, string tag = "", int layer  = 0) {
			Dbg.trc(Dbg.Grp.Init, 3, "extmono ctor");
			go = new GameObject();
			//go.AddComponent(typeof(T));
			go.tag = tag;
			go.layer = layer;
			if(parent != null) {
				go.transform.parent = parent.transform;
			}
			go.name = "ObjInstance:" + go.GetInstanceID().ToString();	//this.GetType().ToString() + ":" + 
			UnityEngine.Object.DontDestroyOnLoad(go);	//TODO: check if needed			
		}

		protected Component AddComponent<TComp>() {
			return go.AddComponent(typeof(TComp)); 
		}

		public virtual void Awake() {
			Dbg.trc(Dbg.Grp.Init, 1, this.GetType().FullName.ToString() + ":Awake has no base class override, you sure you want this?");
		}

		public virtual void Start() {
			Dbg.trc(Dbg.Grp.Init, 1, this.GetType().FullName.ToString() + ":Start has no base class override, you sure you want this?");
		}

		public virtual void Update() {
			Dbg.trc(Dbg.Grp.Init, 1, this.GetType().FullName.ToString() + ":Update has no base class override, you sure you want this?");
		}
		

	}
}

