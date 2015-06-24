using UnityEngine;
using System.Collections;
using System.Threading;
using System.Reflection;

	public delegate IEnumerator MonitorCoroutine<T> (CoroutineData<T> data);

	public static class MonoBehaviourExtension {
		public static CoroutineData<T> StartCoroutine<T> (this MonoBehaviour behaviour, IEnumerator coroutine) {
			return CoroutineData<T>.Start (behaviour, coroutine);
		}

		public static CoroutineData<T> StartCoroutine<T> (this MonoBehaviour behaviour, MonitorCoroutine<T> coroutine) {
			return CoroutineData<T>.Start (behaviour, coroutine);
		}
	}

	public class CoroutineStoppedException : System.ApplicationException {
	}

	public class WaitForWorkerThread {
	}

	public class WaitForMainThread {
	}

	public class WaitIfFrameTime {
		public float MaxFrameTime {
			get;
			protected set;
		}

		public WaitIfFrameTime (float maxFrameTime) {
			MaxFrameTime = maxFrameTime;
		}
	}

	public class CoroutineData<T> {
		Coroutine m_Coroutine;
		T m_Value;
		System.Exception m_Exception;
		volatile bool
			m_Running,
			m_Completed,
			m_Stopped,
			m_ShouldStop;
		Thread m_Thread;
		Object m_ValueLock = new Object (), m_ExceptionLock = new Object ();

		public Coroutine Coroutine {
			get {
				lock (m_Coroutine) {
					return m_Coroutine;
				}
			}
		}

		public T Value {
			get {
				lock (m_ValueLock) {
					System.Exception exception = Exception;
					if (exception != null) {
						throw exception;
					}

					if (Stopped) {
						throw new CoroutineStoppedException ();
					}

					// TODO: Decide if we should do something special if not m_Completed

					return m_Value;
				}
			}
			protected set {
				lock (m_ValueLock) {
					m_Value = value;
				}
			}
		}

		public System.Exception Exception {
			get {
				lock (m_ExceptionLock) {
					return m_Exception;
				}
			}
			protected set {
				lock (m_ExceptionLock) {
					m_Exception = value;
				}
			}
		}

		public bool Running {
			get {
				return m_Running;
			}
			protected set {
				m_Running = value;
			}
		}

		public bool Completed {
			get {
				return m_Completed;
			}
			protected set {
				m_Completed = value;
			}
		}

		public bool Stopped {
			get {
				return m_Stopped;
			}
			protected set {
				m_Stopped = value;
			}
		}

		public bool ShouldStop {
			get {
				return m_ShouldStop;
			}
			protected set {
				m_ShouldStop = value;
			}
		}

		public bool RunningOnMainThread {
			get {
				return m_Thread == null;
			}
		}

		Thread Thread {
			get {
				return m_Thread;
			}
			set {
				if (m_Thread != null && value == null) {
					m_Thread.Abort ();
				}

				m_Thread = value;
			}
		}

		CoroutineData () {
			Running = true;
			Completed = false;
		}

		internal static CoroutineData<T> Start (MonoBehaviour behaviour, IEnumerator coroutine) {
			CoroutineData<T> instance = new CoroutineData<T> ();
			instance.m_Coroutine = behaviour.StartCoroutine (instance.Wrap (coroutine));
			return instance;
		}

		internal static CoroutineData<T> Start (MonoBehaviour behaviour, MonitorCoroutine<T> coroutine) {
			CoroutineData<T> instance = new CoroutineData<T> ();
			instance.m_Coroutine = behaviour.StartCoroutine (instance.Wrap (coroutine (instance)));
			return instance;
		}

		IEnumerator Wrap (IEnumerator coroutine) {
			if (coroutine == null) {
				Exception = new System.ArgumentException ("Coroutine reference is null");
				Running = false;

				yield break;
			}

			while (true) {
				if (Stopped || !Running) {
					Running = false;
					Thread = null;

					yield break;
				}

				if (Thread != null) {
					if (Thread.IsAlive) {
						yield return null;

						continue;
					} else {
						#if DEBUG_THREADING
						Debug.Log ("Worker thread terminated, returning to main thread");
						#endif

						Thread = null;
					}
				}

				try {
					if (!coroutine.MoveNext ()) {
						Running = false;

						yield break;
					}
				} catch (System.Exception e) {
					Exception = e;
					Running = false;

					yield break;
				}

				object current = coroutine.Current;

				if (current != null) {
					if (current is WaitForWorkerThread) {
						#if DEBUG_THREADING
						Debug.Log ("WaitForWorkerThread received, switching to worker thread");
						#endif

						Thread = new Thread (WorkerThread);
						Thread.Start (coroutine);
						yield return null;

						continue;
					} else if (current is WaitForMainThread) {
						Debug.LogWarning ("Received WaitForMainThread while already on main thread");
						continue;
					} else if (current is T) {
						Value = (T)current;
						Running = false;
						Completed = true;

						yield break;
					} else if (current is WaitIfFrameTime) {
						if (Time.realtimeSinceStartup - Time.time > ((WaitIfFrameTime)current).MaxFrameTime) {
							yield return null;
						}

						continue;
					}
				}

				yield return coroutine.Current;
			}
		}

		void WorkerThread (object coroutineObject) {
			IEnumerator coroutine = coroutineObject as IEnumerator;

			if (coroutine == null) {
				Exception = new System.ArgumentException ("Coroutine object passed to thread is null");
				Running = false;

				return;
			}

			#if DEBUG_THREADING
			Debug.Log ("Worker thread running");
			#endif

			while (true) {
				if (Stopped || !Running) {
					#if DEBUG_THREADING
					Debug.Log ("Terminating worker thread on stop signal");
					#endif

					Running = false;

					return;
				}

				try {
					if (!coroutine.MoveNext ()) {
						#if DEBUG_THREADING
						Debug.Log ("Terminating worker thread on coroutine break");
						#endif

						Running = false;

						return;
					}
				} catch (System.Exception e) {
					#if DEBUG_THREADING
					Debug.Log ("Terminating worker thread on exception");
					#endif

					Exception = e;
					Running = false;

					return;
				}

				object current = coroutine.Current;

				if (current != null) {
					if (current is WaitForWorkerThread) {
						Debug.LogWarning ("Received WaitForWorkerThread while already on worker thread");

						continue;
					} else if (current is WaitForMainThread) {
						#if DEBUG_THREADING
						Debug.Log ("WaitForMainThread received, terminating worker thread");
						#endif

						return;
					} else if (current is WaitForSeconds) {
						#if DEBUG_THREADING
						Debug.Log ("Sleeping worker thread");
						#endif

						FieldInfo secondsField = typeof(WaitForSeconds).GetField (
						"m_Seconds",
						BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
						);

						Thread.Sleep (System.TimeSpan.FromSeconds ((float)secondsField.GetValue (current)));

						continue;
					} else if (current is T) {
						#if DEBUG_THREADING
						Debug.Log ("Terminating worker thread on result");
						#endif

						Value = (T)current;
						Running = false;
						Completed = true;

						return;
					} else {
						Debug.LogWarning (string.Format ("Unsupported worker thread yield instruction: {0}", current.GetType ().Name));
					}
				}

				#if DEBUG_THREADING
				Debug.Log ("Yielding worker thread");
				#endif

				Thread.Sleep (1); // TODO: Come up with something nicer here
			}
		}

		public void Stop () {
			Stopped = true;
		}

		public void RequestStop () {
			ShouldStop = true;
		}
	}
