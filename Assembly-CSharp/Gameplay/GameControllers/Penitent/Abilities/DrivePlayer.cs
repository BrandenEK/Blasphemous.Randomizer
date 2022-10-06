using System;
using System.Collections;
using System.Diagnostics;
using CreativeSpore.SmartColliders;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Penitent.States;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Abilities
{
	public class DrivePlayer : Ability
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Core.SimpleEvent OnStartMotion;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Core.SimpleEvent OnStopMotion;

		protected override void OnStart()
		{
			base.OnStart();
			this._penitent = (Penitent)base.EntityOwner;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!base.Casting)
			{
				return;
			}
			eControllerActions eControllerActions = (this._destination.x < base.EntityOwner.transform.position.x) ? 2 : 1;
			if (Mathf.Abs(this._penitent.transform.position.x - this._destination.x) > 0.1f)
			{
				this.SetAnimatorRunning(true);
				this._penitent.PlatformCharacterController.SetActionState(eControllerActions, true);
				this._penitent.SetOrientation((eControllerActions != 1) ? EntityOrientation.Left : EntityOrientation.Right, true, false);
			}
			else
			{
				this._penitent.PlatformCharacterController.SetActionState(eControllerActions, false);
				this._penitent.PlatformCharacterController.PlatformCharacterPhysics.HSpeed = 0f;
				this.SetAnimatorRunning(false);
				this._penitent.SetOrientation(this._finalOrientation, true, false);
				base.StartCoroutine(this.ClampReposition());
			}
		}

		public void MoveToPosition(Vector2 position, EntityOrientation finalOrientation)
		{
			if (base.Casting)
			{
				return;
			}
			this._destination = position;
			this._finalOrientation = finalOrientation;
			base.Cast();
		}

		private IEnumerator ClampReposition()
		{
			do
			{
				this._penitent.transform.position = new Vector2(this._destination.x, base.EntityOwner.transform.position.y);
				yield return null;
			}
			while (Mathf.Abs(this._penitent.transform.position.x - this._destination.x) > Mathf.Epsilon);
			base.StopCast();
			yield break;
		}

		protected override void OnCastStart()
		{
			base.OnCastStart();
			if (this.OnStartMotion != null)
			{
				this.OnStartMotion();
			}
			this._penitent.StateMachine.SwitchState<Driven>();
		}

		protected override void OnCastEnd(float castingTime)
		{
			base.OnCastEnd(castingTime);
			if (this.OnStopMotion != null)
			{
				this.OnStopMotion();
			}
			this._penitent.StateMachine.SwitchState<Playing>();
		}

		private void SetAnimatorRunning(bool running = true)
		{
			base.EntityOwner.Animator.SetBool("RUN_STEP", running);
			base.EntityOwner.Animator.SetBool("RUNNING", running);
		}

		private Penitent _penitent;

		private Vector2 _destination;

		private EntityOrientation _finalOrientation;
	}
}
