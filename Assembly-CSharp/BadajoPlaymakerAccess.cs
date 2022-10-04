using System;
using Framework.Managers;
using Framework.Managers.LevelSingleton.InvertedBell;
using UnityEngine;

public class BadajoPlaymakerAccess : MonoBehaviour
{
	public void PlayBreak()
	{
		BadajoManager badajoManager = UnityEngine.Object.FindObjectOfType<BadajoManager>();
		Core.Logic.CameraManager.ProCamera2DShake.Shake(0.4f, new Vector3(2f, 2f, 0f), 20, 0.25f, 0f, default(Vector3), 0.06f, true);
		Core.Logic.CameraManager.ShockwaveManager.Shockwave(base.transform.position, 1.9f, 0.3f, 1.8f);
		badajoManager.PlayBreak();
	}

	public void PlayReactionLeft()
	{
		BadajoManager badajoManager = UnityEngine.Object.FindObjectOfType<BadajoManager>();
		Core.Logic.CameraManager.ProCamera2DShake.Shake(0.4f, new Vector3(1f, 1f, 0f), 15, 0.25f, 0f, default(Vector3), 0.06f, true);
		Core.Logic.CameraManager.ShockwaveManager.Shockwave(base.transform.position, 1.5f, 0.1f, 2f);
		badajoManager.PlayReactionLeft();
	}

	public void PlayReactionRight()
	{
		BadajoManager badajoManager = UnityEngine.Object.FindObjectOfType<BadajoManager>();
		Core.Logic.CameraManager.ProCamera2DShake.Shake(0.4f, new Vector3(1f, 1f, 0f), 15, 0.25f, 0f, default(Vector3), 0.06f, true);
		Core.Logic.CameraManager.ShockwaveManager.Shockwave(base.transform.position, 1.5f, 0.1f, 2f);
		badajoManager.PlayReactionRight();
	}
}
