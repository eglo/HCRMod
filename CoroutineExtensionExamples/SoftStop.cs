CoroutineData<string> stringCoroutine = StartCoroutine<string> (OtherRoutine);

yield return new WaitForSeconds (1);

stringCoroutine.RequestStop ();

yield return stringCoroutine.Coroutine;

Debug.Log ("Done: " + stringCoroutine.Value);

// ...

IEnumerator OtherRoutine (CoroutineData<string> self)
{
	Debug.Log ("Start");

	float end = Time.time + 3.0f;

	while (Time.time < end)
	{
		yield return new WaitForSeconds (0.1f);

		if (self.ShouldStop)
		{
			Debug.Log ("Stopping");
			yield break;
		}
	}

	Debug.Log ("Completed");
}