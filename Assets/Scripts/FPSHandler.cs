using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSHandler : MonoBehaviour
{
	[SerializeField] int targetFPS = 30;

	float deltaTime = 0.0f;

	//Set the target fps to be 30 (suited for 3D mobile apps)
    private void Start()
    {
		Application.targetFrameRate = targetFPS;
    }

    void Update()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

		if (Application.targetFrameRate != targetFPS)
			Application.targetFrameRate = targetFPS;
	}

	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 50;
		style.normal.textColor = Color.white;
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		GUI.Label(rect, text, style);
	}
}

