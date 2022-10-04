using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

public class TPOFollowerAudioEvents : MonoBehaviour
{
	private void Awake()
	{
		SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
	}

	private void OnPlayerSpawn(Penitent penitent)
	{
		penitent.Audio.Mute = true;
		SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
	}

	public void PlayJumpFX()
	{
		Core.Audio.PlayOneShot("event:/SFX/DEMAKE/DTPOJump", default(Vector3));
	}

	public void PlayAttackFX()
	{
		Core.Audio.PlayOneShot("event:/SFX/DEMAKE/DTPOAttack", default(Vector3));
	}

	public void PlayDashFX()
	{
	}

	public void PlayHurtFX()
	{
		Core.Audio.PlayOneShot("event:/SFX/DEMAKE/DTPOHurt", default(Vector3));
	}

	public void PlayDeathFX()
	{
		Core.Audio.PlayOneShot("event:/SFX/DEMAKE/DTPODeath", default(Vector3));
	}

	public void PlayRespawnFX()
	{
		Core.Audio.PlayOneShot("event:/SFX/DEMAKE/DTPORespawn", default(Vector3));
	}
}
