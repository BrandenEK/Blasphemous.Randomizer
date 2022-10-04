using System;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Stoners.Animator
{
	public class StonerAnimatorBridge : MonoBehaviour
	{
		private void Awake()
		{
			this._stoners = base.GetComponentInParent<Stoners>();
		}

		public void SetBullsEyePos()
		{
			this._stoners.Attack.SetBullsEyeWhenThrow();
		}

		public void ThrowRock()
		{
			this._stoners.Attack.ThrowRock();
		}

		public void PlayStonersRaise()
		{
			if (this._stoners.Status.IsVisibleOnCamera)
			{
				this._stoners.Audio.Raise();
			}
		}

		public void PlayThrowRock()
		{
			this._stoners.Audio.ThrowRock();
		}

		public void PlayPassRock()
		{
			this._stoners.Audio.PassRock();
		}

		public void PlayDamage()
		{
			this._stoners.Audio.Damage();
		}

		public void PlayDeath()
		{
			this._stoners.Audio.Death();
		}

		public void InstantiateStonersGrave()
		{
			if (this.StonersGrave == null)
			{
				return;
			}
			UnityEngine.Object.Instantiate<GameObject>(this.StonersGrave, this._stoners.Animator.transform.position, Quaternion.identity);
			UnityEngine.Object.Destroy(this._stoners.gameObject);
		}

		private Stoners _stoners;

		public GameObject StonersGrave;
	}
}
