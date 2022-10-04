using System;
using System.Collections;
using Framework.Managers;
using Framework.Util;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bosses.Pietat.ThornProjectile
{
	public class PietyProjectileDestroyBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			this._projectile = animator.transform.root.gameObject;
			this._renderer = animator.GetComponent<SpriteRenderer>();
			if (!this._renderer.enabled)
			{
				Singleton<Core>.Instance.StartCoroutine(this.InvokeDestruction());
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (stateInfo.normalizedTime <= 0.95f)
			{
				return;
			}
			if (this._renderer.enabled)
			{
				this._renderer.enabled = false;
				Singleton<Core>.Instance.StartCoroutine(this.InvokeDestruction());
			}
		}

		private void DestroyProjectile()
		{
			if (this._projectile != null)
			{
				UnityEngine.Object.Destroy(this._projectile);
			}
		}

		private IEnumerator InvokeDestruction()
		{
			yield return new WaitForSeconds(3f);
			this.DestroyProjectile();
			yield break;
		}

		private GameObject _projectile;

		private SpriteRenderer _renderer;
	}
}
