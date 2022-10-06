using System;
using System.Collections;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.CommonAttacks;
using Gameplay.GameControllers.Enemies.Projectiles;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Generic.Attacks
{
	public class BossMachinegunShooter : MonoBehaviour
	{
		private void Start()
		{
			PoolManager.Instance.CreatePool(this.muzzleFlashEffect, this.projectileAttack.poolSize);
		}

		public void StartAttack(Transform target)
		{
			this._currentTarget = target;
			base.StartCoroutine(this.MachinegunCoroutine());
		}

		private IEnumerator MachinegunCoroutine()
		{
			float durationCounter = 0f;
			this.machinegunAnimator.SetBool("FIRE", true);
			while (durationCounter < this.maxDuration)
			{
				float nValue = this.fireRateChangeCurve.Evaluate(durationCounter / this.maxDuration);
				this._currentDisp = this.dispersionAtMaxSpeed * nValue;
				this.machinegunAnimator.speed = Mathf.Lerp(this.minAnimSpeed, this.maxAnimSpeed, nValue);
				durationCounter += Time.deltaTime;
				yield return null;
			}
			this.machinegunAnimator.SetBool("FIRE", false);
			yield break;
		}

		public void StopMachinegun()
		{
			this.machinegunAnimator.SetBool("FIRE", false);
			base.StopAllCoroutines();
		}

		public void OnMachinegunRevolution()
		{
			if (this.machinegunAnimator.GetBool("FIRE"))
			{
				this.Shoot(this._currentDisp, this.useVerticalDispersion);
			}
		}

		private void Shoot(float dispersion, bool verticalDispersion = false)
		{
			Vector2 vector = this._currentTarget.transform.position + Vector2.up * 1.4f;
			if (verticalDispersion)
			{
				vector += Vector2.up * Random.Range(-1f, 1f) * dispersion;
			}
			else
			{
				vector += new Vector2(Random.Range(-1f, 1f), 0f) * dispersion;
			}
			Vector2 dir = vector - base.transform.position;
			StraightProjectile straightProjectile = this.projectileAttack.Shoot(dir, dir.normalized, 1f);
			AcceleratedProjectile component = straightProjectile.GetComponent<AcceleratedProjectile>();
			component.SetAcceleration(dir.normalized * 10f);
			if (this.useBounceBackData && this.bouncebackOwner != null)
			{
				component.SetBouncebackData(this.bouncebackOwner, this.bouncebackOffset, 4);
			}
			PoolManager.Instance.ReuseObject(this.muzzleFlashEffect, base.transform.position, Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * 57.29578f), false, 1);
		}

		public float maxAnimSpeed = 3f;

		public float minAnimSpeed = 1f;

		public float maxDuration = 7f;

		public AnimationCurve fireRateChangeCurve;

		public BossStraightProjectileAttack projectileAttack;

		public GameObject muzzleFlashEffect;

		public Animator machinegunAnimator;

		private float _currentDisp;

		private Transform _currentTarget;

		public bool useBounceBackData;

		public Transform bouncebackOwner;

		public Vector2 bouncebackOffset;

		public float dispersionAtMaxSpeed = 1f;

		public bool useVerticalDispersion;
	}
}
