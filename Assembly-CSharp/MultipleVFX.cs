using System;
using System.Collections;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

public class MultipleVFX : MonoBehaviour
{
	private void Start()
	{
		PoolManager.Instance.CreatePool(this.vfx.gameObject, this.number);
	}

	[Button("Test spawn", ButtonSizes.Small)]
	public void Play()
	{
		base.StartCoroutine(this.SpawnVFX());
	}

	private IEnumerator SpawnVFX()
	{
		for (int i = 0; i < this.number; i++)
		{
			Vector2 p = base.transform.position + this.offset + new Vector2(UnityEngine.Random.Range(-this.range, this.range), UnityEngine.Random.Range(-this.range, this.range));
			PoolManager.Instance.ReuseObject(this.vfx.gameObject, p, Quaternion.identity, false, 1);
			yield return new WaitForSeconds(this.delayBetweenEffects);
		}
		yield break;
	}

	public SimpleVFX vfx;

	public int number;

	public float delayBetweenEffects;

	public float range;

	public Vector2 offset;
}
