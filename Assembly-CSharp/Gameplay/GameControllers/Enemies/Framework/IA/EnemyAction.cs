using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Framework.IA
{
	public class EnemyAction
	{
		public EnemyAction()
		{
			this.Finished = true;
			this.waitForCompletion = new WaitUntilActionFinishes(this);
			this.waitForCallback = new WaitUntilActionCustomCallback(this);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EnemyAction.EnemyActionCallback OnActionStarts;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EnemyAction.EnemyActionCallback OnActionIsStopped;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EnemyAction.EnemyActionCallback OnActionEnds;

		public bool Finished { get; set; }

		public bool CallbackCalled { get; set; }

		public EnemyAction StartAction(EnemyBehaviour e)
		{
			this.id = EnemyAction.actionsId;
			EnemyAction.actionsId++;
			UnityEngine.Debug.Log(string.Format("<color=blue>[{2:D6}]/---------- EA START: ID{0}_{1}</color>", this.id, this.ToString(), Time.frameCount));
			this.Finished = false;
			this.CallbackCalled = false;
			if (this.OnActionStarts != null)
			{
				this.OnActionStarts(this);
			}
			this.owner = e;
			this.DoOnStart();
			this.currentCoroutine = e.StartCoroutine(this.BaseCoroutine());
			return this;
		}

		protected void Callback()
		{
			this.CallbackCalled = true;
			this.DoOnCallback();
		}

		protected void FinishAction()
		{
			UnityEngine.Debug.Log(string.Format("<color=green>[{2:D6}]\\---------- EA FINISH: ID{0}_{1}</color>", this.id, this.ToString(), Time.frameCount));
			this.Finished = true;
			this.DoOnEnd();
			if (this.OnActionEnds != null)
			{
				this.OnActionEnds(this);
			}
		}

		public void StopAction()
		{
			if (this.Finished || this.currentCoroutine == null)
			{
				return;
			}
			this.Finished = true;
			UnityEngine.Debug.Log(string.Format("<color=red>[{2:D6}]/----------!EA STOPPED: ID{0}_{1}</color>", this.id, this.ToString(), Time.frameCount));
			this.DoOnStop();
			if (this.OnActionIsStopped != null)
			{
				this.OnActionIsStopped(this);
			}
			this.owner.StopCoroutine(this.currentCoroutine);
		}

		protected virtual void DoOnStart()
		{
		}

		protected virtual void DoOnEnd()
		{
		}

		protected virtual void DoOnCallback()
		{
		}

		protected virtual void DoOnStop()
		{
		}

		protected virtual IEnumerator BaseCoroutine()
		{
			yield return null;
			yield break;
		}

		public override string ToString()
		{
			return base.GetType().Name;
		}

		protected EnemyBehaviour owner;

		protected Coroutine currentCoroutine;

		public CustomYieldInstruction waitForCompletion;

		public CustomYieldInstruction waitForCallback;

		private int id;

		public static int actionsId;

		public delegate void EnemyActionCallback(EnemyAction e);
	}
}
