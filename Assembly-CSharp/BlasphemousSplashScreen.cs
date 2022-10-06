using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class BlasphemousSplashScreen : MonoBehaviour
{
	private void Awake()
	{
		VideoPlayer component = base.GetComponent<VideoPlayer>();
		component.loopPointReached += delegate(VideoPlayer source)
		{
			SceneManager.LoadScene(this.postSplashScene, 0);
		};
		component.prepareCompleted += delegate(VideoPlayer source)
		{
			source.Play();
		};
		component.Prepare();
	}

	public string postSplashScene = "Landing";
}
