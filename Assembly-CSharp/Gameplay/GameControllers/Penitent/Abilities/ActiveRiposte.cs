using System;
using System.Collections;
using Framework.FrameworkCore;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Abilities
{
	public class ActiveRiposte : Ability
	{
		public bool IsTriggeredRiposte { get; set; }

		protected override void OnStart()
		{
			base.OnStart();
			if (this.RiposteEffectObject)
			{
				PoolManager.Instance.CreatePool(this.RiposteEffectObject, this.RiposteEffectPoolSize);
			}
			this._unsetActiveRiposteDelay = new WaitForSeconds(1f);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!base.Casting)
			{
				return;
			}
			this._opportunityOffsetLapse -= Time.deltaTime;
			if (this._opportunityOffsetLapse <= 0f)
			{
				this._opportunityWindowLapse -= Time.deltaTime;
				if (this._opportunityWindowLapse >= 0f)
				{
					if (this.AttackTrigger)
					{
						this.TriggerActiveRiposte();
					}
				}
				else
				{
					base.StopCast();
				}
			}
			else if (this.AttackTrigger)
			{
				base.StopCast();
			}
		}

		protected override void OnCastStart()
		{
			base.OnCastStart();
			this.ResetWindowOpportunityTimers();
		}

		protected override void OnCastEnd(float castingTime)
		{
			base.OnCastEnd(castingTime);
		}

		private void ResetWindowOpportunityTimers()
		{
			this._opportunityOffsetLapse = this.OpportunityWindowOffset;
			this._opportunityWindowLapse = base.EntityOwner.Stats.ActiveRiposteWindow.Final;
		}

		private bool AttackTrigger
		{
			get
			{
				return Core.Logic.Penitent.PlatformCharacterInput.Rewired.GetButtonDown(5);
			}
		}

		private void TriggerActiveRiposte()
		{
			if (this.IsTriggeredRiposte)
			{
				return;
			}
			this.IsTriggeredRiposte = true;
			base.StartCoroutine(this.UnsetActiveRiposte());
		}

		private IEnumerator UnsetActiveRiposte()
		{
			yield return this._unsetActiveRiposteDelay;
			if (this.IsTriggeredRiposte)
			{
				this.IsTriggeredRiposte = !this.IsTriggeredRiposte;
			}
			yield break;
		}

		public void MakeRiposte()
		{
			this.AddProgressToAC28();
			this.InstantiateRiposteEffect();
		}

		private void AddProgressToAC28()
		{
			if (Core.AchievementsManager.Achievements["AC28"].IsGranted())
			{
				return;
			}
			Core.AchievementsManager.Achievements["AC28"].AddProgress(20f);
		}

		private void InstantiateRiposteEffect()
		{
			if (this.RiposteEffectObject == null)
			{
				return;
			}
			PoolManager.Instance.ReuseObject(this.RiposteEffectObject, this.GetRiposteEffectPosition(), Quaternion.identity, false, 1);
		}

		private Vector2 GetRiposteEffectPosition()
		{
			Vector3 position = base.EntityOwner.transform.position;
			EntityOrientation orientation = base.EntityOwner.Status.Orientation;
			float x = (orientation != EntityOrientation.Left) ? (position.x + this.RiposteEffectOffset.x) : (position.x - this.RiposteEffectOffset.x);
			Vector2 result = new Vector2(x, position.y + this.RiposteEffectOffset.y);
			return result;
		}

		[FoldoutGroup("Active Riposte Settings", 0)]
		[Range(0f, 2f)]
		public float OpportunityWindowOffset;

		private float _opportunityOffsetLapse;

		[FoldoutGroup("Active Riposte Settings", 0)]
		public Vector2 RiposteEffectOffset;

		[FoldoutGroup("Active Riposte Settings", 0)]
		public GameObject RiposteEffectObject;

		[FoldoutGroup("Active Riposte Settings", 0)]
		public int RiposteEffectPoolSize = 1;

		private float _opportunityWindowLapse;

		private WaitForSeconds _unsetActiveRiposteDelay;

		private const int TotalNumberOfRighteousRipostesForAc28 = 5;
	}
}
