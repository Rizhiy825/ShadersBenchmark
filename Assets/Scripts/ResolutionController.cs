using System;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionController : MonoBehaviour
{
	public static readonly Dictionary<int, Tuple<int, int>> ResolutionMultiplier = FillResolutionDictionary();
	public static int DefaultResolutionIndex => 3;

	private static Dictionary<int, Tuple<int, int>> FillResolutionDictionary()
	{
		return new Dictionary<int, Tuple<int, int>>()
		{
			{0, new Tuple<int, int>(679, 382)},
			{1, new Tuple<int, int>(960, 540)},
			{2, new Tuple<int, int>(1357, 764)},
			{3, new Tuple<int, int>(1920, 1080)},
			{4, new Tuple<int, int>(2716, 1527)},
			{5, new Tuple<int, int>(3840, 2160)}
		};
	}

	private int rezWidth;
	private int rezHeight;

	private Camera camera;
	private RenderTexture cameraTarget;

	void Start()
	{
		UpdateResolution(DefaultResolutionIndex);
		camera = GetComponent<Camera>();
	}

	public void UpdateResolution(int index)
	{
		rezWidth = ResolutionMultiplier[index].Item1;
		rezHeight = ResolutionMultiplier[index].Item2;
	}
	
	public void UpdateResolution(int width, int height)
	{
		rezWidth = width;
		rezHeight = height;
	}

	void OnPreCull()
	{
		cameraTarget = RenderTexture.GetTemporary(rezWidth, rezHeight, 24);
		camera.targetTexture = cameraTarget;
	}

	void OnPostRender()
	{
		camera.targetTexture = null;
		Graphics.Blit(cameraTarget, null as RenderTexture);
		RenderTexture.ReleaseTemporary(cameraTarget);
	}
}
