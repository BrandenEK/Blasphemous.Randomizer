using System;
using System.Collections.Generic;
using Gameplay.GameControllers.Bosses.PietyMonster.IA;
using Gameplay.GameControllers.Bosses.PietyMonster.ThornProjectile;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PietyMonster.Attack
{
	public class PietySpitAttack : EnemyAttack
	{
		public GameObject Target { get; set; }

		public int CurrentSpitAmount { get; set; }

		public float SpitDamage { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this._pietyMonster = (PietyMonster)base.EntityOwner;
		}

		protected override void OnStart()
		{
			base.OnStart();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.SetCurrentSpitAmount();
			if (this.PietyMonsterBehaviour.PlayerSeen && this.Target == null)
			{
				this.Target = this._pietyMonster.PietyRootsManager.Target;
			}
			if (!this.Target)
			{
				return;
			}
			if (Vector2.Distance(this._pietyMonster.GetPosition(), this.Target.transform.position) >= this.SpitDistance && this.PietyMonsterBehaviour.CanSpit)
			{
				if (!this.PietyMonsterBehaviour.Spiting)
				{
					this.PietyMonsterBehaviour.Spiting = true;
				}
			}
			else if (this.PietyMonsterBehaviour.Spiting)
			{
				this.PietyMonsterBehaviour.Spiting = false;
			}
		}

		public void Spit()
		{
			if (this.SpitPrefab != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.SpitPrefab, this._pietyMonster.SpitingMouth.GetPosition(), Quaternion.identity);
				ThornProjectile component = gameObject.GetComponent<ThornProjectile>();
				component.GetComponentInChildren<IProjectileAttack>().SetProjectileWeaponDamage((int)this.SpitDamage);
				component.Target = this.Target.transform;
				component.SetOwner(this._pietyMonster);
				float horSpitPosition = this.GetHorSpitPosition();
				Vector2 v = new Vector2(horSpitPosition, this._pietyMonster.PietyRootsManager.Collider.bounds.min.y);
				component.Throw(v);
			}
		}

		private void SetCurrentSpitAmount()
		{
			float missingRatio = this._pietyMonster.Stats.Life.MissingRatio;
			if (missingRatio > 0.75f)
			{
				this.CurrentSpitAmount = 1;
			}
			else if (missingRatio > 0.5f)
			{
				this.CurrentSpitAmount = 2;
			}
			else if (missingRatio >= 0f)
			{
				this.CurrentSpitAmount = 3;
			}
		}

		public void RefillSpitPositions()
		{
			this.HorPositions.Clear();
			Vector2 vector = new Vector2(this.Target.transform.position.x, this.Target.transform.position.y);
			for (int i = 0; i < this.CurrentSpitAmount; i++)
			{
				if (i == 0)
				{
					this.HorPositions.Add(vector.x);
				}
				else if (i % 2 != 0)
				{
					this.HorPositions.Add(vector.x + (float)i * UnityEngine.Random.Range(1f, 2.5f));
				}
				else
				{
					this.HorPositions.Add(vector.x - (float)i * UnityEngine.Random.Range(1f, 2.5f));
				}
			}
		}

		public float GetHorSpitPosition()
		{
			float result = this.Target.transform.position.x;
			if (this.HorPositions.Count > 0)
			{
				int index = UnityEngine.Random.Range(0, this.HorPositions.Count);
				result = this.HorPositions[index];
				this.HorPositions.RemoveAt(index);
			}
			return result;
		}

		private PietyMonster _pietyMonster;

		public PietyMonsterBehaviour PietyMonsterBehaviour;

		public int Spitsthrown;

		public GameObject SpitPrefab;

		public List<float> HorPositions = new List<float>();

		[Tooltip("Minimun spit distance expressed in Unity Units")]
		public float SpitDistance = 6f;
	}
}
