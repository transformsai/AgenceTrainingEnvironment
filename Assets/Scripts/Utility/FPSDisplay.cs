using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
	private float deltaTime = 0.0f;
	private float fixedUpdateCount = 0;
	private float updateFixedUpdateCountPerSecond;

	private void Awake()
	{
		StartCoroutine(Loop());
	}
	void Update()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
	}

	void FixedUpdate()
	{
		fixedUpdateCount += 1;
	}

	IEnumerator Loop()
	{
		while (true)
		{
			yield return new WaitForSecondsRealtime(1);
			updateFixedUpdateCountPerSecond = fixedUpdateCount;
			fixedUpdateCount = 0;
		}
	}

	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		GUI.Label(new Rect(0, 15, 200, 50), "FixedUpdate Per Second: " + updateFixedUpdateCountPerSecond.ToString(), style);
		GUI.Label(rect, text, style);
	}
}