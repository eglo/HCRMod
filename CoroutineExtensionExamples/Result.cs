yield return StartCoroutine<int> (TestRoutine ()).Coroutine;

CoroutineData<int> coroutine = StartCoroutine<int> (TestRoutine ());
yield return coroutine.Coroutine;

Debug.Log ("Done: " + coroutine.Value);

// ...

IEnumerator TestRoutine ()
{
	Debug.Log ("Start");
	yield return new WaitForSeconds (1);
	Debug.Log ("Result");
	yield return 42;
}