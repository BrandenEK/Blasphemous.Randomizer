using System;
using Gameplay.GameControllers.Entities;
using Rewired;
using UnityEngine;

namespace Framework.FrameworkCore
{
	public class Trait : MonoBehaviour
	{
		public Entity EntityOwner { get; set; }

		private protected Player RewiredInput { protected get; private set; }

		protected virtual void OnAwake()
		{
		}

		protected virtual void OnStart()
		{
		}

		protected virtual void OnTraitEnable()
		{
		}

		protected virtual void OnTraitDisable()
		{
		}

		protected virtual void OnTraitDestroy()
		{
		}

		protected virtual void OnUpdate()
		{
		}

		protected virtual void OnLateUpdate()
		{
		}

		protected virtual void OnFixedUpdate()
		{
		}

		private void Awake()
		{
			this.EntityOwner = base.GetComponentInParent<Entity>();
			if (this.EntityOwner != null)
			{
				this.OnAwake();
			}
		}

		private void Start()
		{
			if (this.EntityOwner == null)
			{
				Debug.LogError(base.GetType() + " has no Entity. Traits won't execute.");
			}
			if (this.EntityOwner != null)
			{
				this.OnStart();
			}
		}

		private void OnEnable()
		{
			if (this.EntityOwner != null)
			{
				this.OnTraitEnable();
			}
		}

		private void OnDisable()
		{
			if (this.EntityOwner != null)
			{
				this.OnTraitDisable();
			}
		}

		private void Update()
		{
			if (this.EntityOwner != null)
			{
				this.OnUpdate();
			}
		}

		private void LateUpdate()
		{
			if (this.EntityOwner != null)
			{
				this.OnLateUpdate();
			}
		}

		private void FixedUpdate()
		{
			if (this.EntityOwner != null)
			{
				this.OnFixedUpdate();
			}
		}
	}
}
