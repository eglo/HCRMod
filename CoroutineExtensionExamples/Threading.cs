Debug.Log ("Starting threaded coroutine");

CoroutineData<bool> threadedCoroutine = StartCoroutine<bool> (ThreadedRoutine ());

yield return threadedCoroutine.Coroutine;

Debug.Log ("Done");

// ...

IEnumerator ThreadedRoutine ()
{
	LogThreadInfo ();

	Debug.Log ("Going to worker thread and sleeping for a bit");

	yield return new WaitForWorkerThread ();

	// Now on worker thread

	yield return new WaitForSeconds (10.0f);

	Debug.Log ("Going back");

	yield return new WaitForMainThread ();

	// Now on main thread

	yield return true;
}