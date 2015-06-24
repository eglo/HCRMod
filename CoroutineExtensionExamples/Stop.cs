yield return StartCoroutine<int> (TestRoutine ()).Coroutine;

CoroutineData<int> coroutine = StartCoroutine<int> (TestRoutine ());
coroutine.Stop ();
yield return coroutine.Coroutine;

Debug.Log ("Done: " + coroutine.Value);

// ...

IEnumerator TestRoutine ()
{
	Debug.Log ("Start");
	yield return null;
	yield return null;
	Debug.Log ("Result");
	yield return 42;
}