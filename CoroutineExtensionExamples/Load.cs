Debug.Log ("Starting balanced coroutine");
yield return StartCoroutine<bool> (BalancedCoroutine ()).Coroutine;
Debug.Log ("Done");

// ...

IEnumerator BalancedCoroutine ()
{
	Debug.Log (string.Format ("Start, frame={0}, time={1}, realtime={2}", Time.frameCount, Time.time, Time.realtimeSinceStartup));
	int iterations = 0, startFrame = Time.frameCount;
	WaitIfFrameTime wait = new WaitIfFrameTime (1);
	while (Time.frameCount == startFrame)
	{
		++iterations;
		yield return wait;
	}
	Debug.Log (string.Format ("Stop, frame={0}, time={1}, realtime={2}, iterations={3}", Time.frameCount, Time.time, Time.realtimeSinceStartup, iterations));
}