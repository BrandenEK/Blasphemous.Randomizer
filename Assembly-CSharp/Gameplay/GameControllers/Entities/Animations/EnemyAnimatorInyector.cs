using System;
using UnityEngine;

namespace Gameplay.GameControllers.Entities.Animations
{
	[RequireComponent(typeof(Animator))]
	public class EnemyAnimatorInyector : MonoBehaviour
	{
		public Animator EntityAnimator { get; protected set; }

		private void Awake()
		{
			if (this.OwnerEntity == null)
			{
				Debug.LogError("Any or all of the properties have not been initialized");
			}
			this.OnAwake();
		}

		protected virtual void OnAwake()
		{
		}

		private void Start()
		{
			if (this.OwnerEntity != null)
			{
				this.EntityAnimator = this.OwnerEntity.Animator;
			}
			this.OnStart();
		}

		protected virtual void OnStart()
		{
		}

		private void Update()
		{
			this.OnUpdate();
		}

		protected virtual void OnUpdate()
		{
		}

		public void SetVulnerable(EnemyAnimatorInyector.BooleanParameter parameter)
		{
			Enemy enemy = (Enemy)this.OwnerEntity;
			if (enemy == null)
			{
				return;
			}
			enemy.IsVulnerable = (parameter == EnemyAnimatorInyector.BooleanParameter.True);
		}

		[SerializeField]
		protected Entity OwnerEntity;

		public enum BooleanParameter
		{
			True,
			False
		}
	}
}
