using System;
using System.Collections;
using Maikel.SteeringBehaviors;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Framework.IA
{
	public class MoveToPointUsingAgent_EnemyAction : EnemyAction
	{
		public IEnumerator WaitUntilClose(Transform t, Vector3 target, float distance = 0.01f)
		{
			while (Vector2.Distance(t.position, target) > distance)
			{
				yield return null;
			}
			yield break;
		}

		public EnemyAction StartAction(EnemyBehaviour e, AutonomousAgent _agent, Vector2 _target, float closeDistance = 2f)
		{
			this.point = _target;
			this.agent = _agent;
			this.arriveBehaviour = this.agent.GetComponent<Arrive>();
			if (this.arriveBehaviour == null)
			{
				Debug.LogError("Movement requires an Arrive component linked to the autonomous agent");
			}
			this.closeDistance = closeDistance;
			this.arriveBehaviour.target = this.point;
			this.agent.enabled = true;
			return base.StartAction(e);
		}

		protected override void DoOnStart()
		{
			base.DoOnStart();
		}

		protected override void DoOnStop()
		{
			base.DoOnStop();
			this.agent.enabled = false;
		}

		protected override IEnumerator BaseCoroutine()
		{
			yield return this.WaitUntilClose(this.owner.transform, this.point, this.closeDistance);
			base.Callback();
			yield return this.WaitUntilClose(this.owner.transform, this.point, 0.01f);
			this.agent.enabled = false;
			base.FinishAction();
			yield break;
		}

		private Vector2 point;

		private Arrive arriveBehaviour;

		private AutonomousAgent agent;

		private float closeDistance;
	}
}
