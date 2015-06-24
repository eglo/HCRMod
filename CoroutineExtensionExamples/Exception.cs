yield return StartCoroutine<int> (TestRoutine ()).Coroutine;

CoroutineData<int> coroutine = StartCoroutine<int> (TestRoutine ());
yield return coroutine.Coroutine;

try
{
	Debug.Log ("Done: " + coroutine.Value);
}
catch (Exception e)
{
	Debug.LogError ("Exception during coroutine execution: " + e);
}

// ...

IEnumerator TestRoutine ()
{
	Debug.Log ("Start");
	yield return new WaitForSeconds (1);
	
	throw new ApplicationException ("Boom!");
	
	Debug.Log ("Result");
	yield return 42;
}