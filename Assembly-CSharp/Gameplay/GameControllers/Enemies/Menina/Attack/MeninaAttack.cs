using System;
using System.Collections;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Gameplay.GameControllers.Penitent.Gizmos;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Menina.Attack
{
	public class MeninaAttack : EnemyAttack
	{
		public RootMotionDriver GroundWaveRoot { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			base.CurrentEnemyWeapon = base.GetComponent<Weapon>();
			this._spawningLapseWaiting = new WaitForSeconds(this.WavesSpawningLapse);
			this._endOfFrameWaiting = new WaitForEndOfFrame();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.GroundWaveRoot = base.EntityOwner.GetComponentInChildren<RootMotionDriver>();
			if (this.GroundWave != null)
			{
				PoolManager.Instance.CreatePool(this.GroundWave, this.WavesAmount * 2);
			}
		}

		private Hit GetHit
		{
			get
			{
				return new Hit
				{
					AttackingEntity = base.EntityOwner.gameObject,
					DamageAmount = base.EntityOwner.Stats.Strength.Final,
					DamageType = this.DamageType,
					Force = this.Force,
					HitSoundId = this.HitSound,
					Unnavoidable = true
				};
			}
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
			base.CurrentEnemyWeapon.Attack(this.GetHit);
			if (this.SpawnWaves)
			{
				base.StartCoroutine(this.SpawnGroundWaves());
			}
		}

		public void StopAttack()
		{
			base.StopCoroutine(this.SpawnGroundWaves());
		}

		private IEnumerator SpawnGroundWaves()
		{
			if (!this.GroundWave)
			{
				yield break;
			}
			float spacing = 0f;
			for (int i = 0; i < this.WavesAmount; i++)
			{
				Vector3 position = this.GroundWavePosition;
				if (base.EntityOwner.Status.Orientation == EntityOrientation.Right)
				{
					position.x += spacing;
				}
				else
				{
					position.x -= spacing;
				}
				PoolManager.ObjectInstance go = PoolManager.Instance.ReuseObject(this.GroundWave, position, Quaternion.identity, false, 1);
				MeninaGroundWave groundWave = go.GameObject.GetComponentInChildren<MeninaGroundWave>();
				groundWave.SetOwner(base.EntityOwner);
				this._meninaGroundWaves.Add(groundWave);
				spacing += this.GroundWavesSpacing;
				yield return this._endOfFrameWaiting;
			}
			base.StartCoroutine(this.TriggerGroundWaves());
			yield break;
		}

		private Vector3 GroundWavePosition
		{
			get
			{
				return (base.EntityOwner.Status.Orientation != EntityOrientation.Right) ? this.GroundWaveRoot.ReversePosition : this.GroundWaveRoot.transform.position;
			}
		}

		private IEnumerator TriggerGroundWaves()
		{
			List<MeninaGroundWave> groundWavesCopy = new List<MeninaGroundWave>(this._meninaGroundWaves);
			foreach (MeninaGroundWave groundWaves in groundWavesCopy)
			{
				groundWaves.TriggerWave();
				yield return this._spawningLapseWaiting;
			}
			this._meninaGroundWaves.Clear();
			yield break;
		}

		[FoldoutGroup("Ground Wave Attack Settings", true, 0)]
		public GameObject GroundWave;

		[FoldoutGroup("Ground Wave Attack Settings", true, 0)]
		public int WavesAmount = 3;

		[FoldoutGroup("Ground Wave Attack Settings", true, 0)]
		public float WavesSpawningLapse = 1f;

		[FoldoutGroup("Ground Wave Attack Settings", true, 0)]
		public float GroundWavesSpacing = 2f;

		[FoldoutGroup("Ground Wave Attack Settings", true, 0)]
		public bool SpawnWaves = true;

		private List<MeninaGroundWave> _meninaGroundWaves = new List<MeninaGroundWave>();

		private WaitForSeconds _spawningLapseWaiting;

		private WaitForEndOfFrame _endOfFrameWaiting;
	}
}
