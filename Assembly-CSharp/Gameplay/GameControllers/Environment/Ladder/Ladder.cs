using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.Ladder
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class Ladder : MonoBehaviour
	{
		private void Start()
		{
			this._penitent = Core.Logic.Penitent;
			this._ladderBoxCollider = base.GetComponent<BoxCollider2D>();
		}

		private void Update()
		{
			this.SetLadderClimbable();
		}

		protected void SetLadderClimbable()
		{
			if (this._ladderBoxCollider == null)
			{
				Log.Error("Ladder null collider.", null);
				return;
			}
			if (this._penitent != null && this._penitent.IsJumpingOff)
			{
				this._deltaRecoverTime = 0f;
				base.gameObject.layer = LayerMask.NameToLayer("Default");
				if (this._ladderBoxCollider.enabled)
				{
					this._ladderBoxCollider.enabled = false;
				}
			}
			else
			{
				this._deltaRecoverTime += Time.deltaTime;
				if (this._deltaRecoverTime >= 0.5f)
				{
					base.gameObject.layer = LayerMask.NameToLayer("Ladder");
					if (!this._ladderBoxCollider.enabled)
					{
						this._ladderBoxCollider.enabled = true;
					}
				}
			}
		}

		private Penitent _penitent;

		private float _deltaRecoverTime;

		private BoxCollider2D _ladderBoxCollider;

		private const float RecoverTime = 0.5f;
	}
}
