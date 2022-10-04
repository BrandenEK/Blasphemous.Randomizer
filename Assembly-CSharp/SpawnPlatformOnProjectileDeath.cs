using System;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.BellGhost;
using UnityEngine;

public class SpawnPlatformOnProjectileDeath : MonoBehaviour
{
	private void Start()
	{
		PoolManager.Instance.CreatePool(this.platform, 2);
		ProjectileWeapon component = base.GetComponent<ProjectileWeapon>();
		this.results = new RaycastHit2D[1];
		component.OnProjectileDeath += this.P_OnProjectileDeath;
	}

	private void P_OnProjectileDeath(ProjectileWeapon obj)
	{
		bool flag = false;
		Vector2 pointBelow = this.GetPointBelow(base.transform.position, out flag);
		if (this.onlyOnGround && flag)
		{
			this.SpawnPlatform(pointBelow);
		}
	}

	public void SpawnPlatform(Vector2 p)
	{
		PoolManager.Instance.ReuseObject(this.platform, p, Quaternion.identity, false, 1);
	}

	private Vector2 GetPointBelow(Vector2 p, out bool hit)
	{
		if (Physics2D.RaycastNonAlloc(p, Vector2.down, this.results, 100f, this.groundSnapLayerMask) > 0)
		{
			Debug.DrawLine(p, this.results[0].point, Color.cyan, 5f);
			hit = true;
			return this.results[0].point + Vector2.up * this.heightOffset;
		}
		hit = false;
		return p;
	}

	public GameObject platform;

	public LayerMask groundSnapLayerMask;

	public float heightOffset = 0.5f;

	private RaycastHit2D[] results;

	public bool onlyOnGround = true;
}
