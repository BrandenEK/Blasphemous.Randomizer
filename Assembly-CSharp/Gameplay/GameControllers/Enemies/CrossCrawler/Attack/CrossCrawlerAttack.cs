using System;
using System.Collections;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Menina.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent.Gizmos;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.CrossCrawler.Attack
{
	public class CrossCrawlerAttack : EnemyAttack
	{
		public RootMotionDriver GroundWaveRoot { get; set; }

		private Vector3 GroundWavePosition
		{
			get
			{
				return (base.EntityOwner.Status.Orientation != EntityOrientation.Right) ? this.GroundWaveRoot.ReversePosition : this.GroundWaveRoot.transform.position;
			}
		}

		protected override void OnAwake()
		{
			base.OnAwake();
			base.CurrentEnemyWeapon = base.GetComponentInChildren<CrossCrawlerWeapon>();
			this._waitSecondsQuake = new WaitForSeconds(this.quakeSeconds);
			this._waitSecondsDodge = new WaitForSeconds(this.dodgeOpportunitySeconds);
		}

		protected override void OnStart()
		{
			base.OnStart();
			this._weaponHit = new Hit
			{
				AttackingEntity = base.EntityOwner.gameObject,
				DamageAmount = base.EntityOwner.Stats.Strength.Final,
				DamageType = this.DamageType,
				Force = this.Force,
				HitSoundId = this.HitSound,
				Unnavoidable = false
			};
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
			base.CurrentEnemyWeapon.Attack(this._weaponHit);
		}

		private void CreateGroundwave()
		{
			Vector3 groundWavePosition = this.GroundWavePosition;
			PoolManager.ObjectInstance objectInstance = PoolManager.Instance.ReuseObject(this.GroundWave, groundWavePosition, Quaternion.identity, false, 1);
			this._groundWave = objectInstance.GameObject.GetComponentInChildren<MeninaGroundWave>();
			this._groundWave.SetOwner(base.EntityOwner);
			base.StartCoroutine(this.QuakeBelowPlayerCoroutine());
		}

		private IEnumerator QuakeBelowPlayerCoroutine()
		{
			Core.Logic.CameraManager.ProCamera2DShake.Shake(this.quakeSeconds, new Vector2(0.1f, 3.4f), 40, 0.1f, -1f, default(Vector3), 0.1f, false);
			Debug.Log("QUAKE START");
			yield return this._waitSecondsQuake;
			Debug.Log("QUAKE END ");
			Vector2 targetPos = Core.Logic.Penitent.transform.position;
			bool groundExists = false;
			targetPos = GameplayUtils.GetGroundPosition(targetPos, this.groundWaveLayerMask, out groundExists);
			this._groundWave.transform.position = targetPos;
			Core.Logic.CameraManager.ProCamera2DShake.ShakeUsingPreset("PietyStomp");
			yield return this._waitSecondsDodge;
			this._groundWave.TriggerWave();
			yield break;
		}

		private Hit _weaponHit;

		public CrossCrawler crossCrawler;

		[FoldoutGroup("Ground Wave Attack Settings", true, 0)]
		public GameObject GroundWave;

		public float quakeSeconds;

		public float quakeVibrationStrenght;

		public float dodgeOpportunitySeconds;

		public LayerMask groundWaveLayerMask;

		private WaitForSeconds _waitSecondsQuake;

		private WaitForSeconds _waitSecondsDodge;

		private MeninaGroundWave _groundWave;
	}
}
