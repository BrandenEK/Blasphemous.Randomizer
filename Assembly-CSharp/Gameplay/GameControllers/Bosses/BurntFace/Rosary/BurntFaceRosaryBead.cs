using System;
using System.Collections;
using System.Collections.Generic;
using DamageEffect;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Environment.Traps.Turrets;
using Maikel.StatelessFSM;
using Plugins.Maikel.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BurntFace.Rosary
{
	public class BurntFaceRosaryBead : MonoBehaviour
	{
		private void Awake()
		{
			this.stBeam = new BurntFaceRosaryBead_StBeam();
			this.stProjectile = new BurntFaceRosaryBead_StProjectile();
			this.stInactive = new BurntFaceRosaryBead_StInactive();
			this.stCharging = new BurntFaceRosaryBead_StCharging();
			this.stActive = new BurntFaceRosaryBead_StActive();
			this.stHidden = new BurntFaceRosaryBead_StHidden();
			this.stDestroyed = new BurntFaceRosaryBead_StDestroyed();
			this.fsm = new StateMachine<BurntFaceRosaryBead>(this, this.stInactive, null, null);
		}

		private void Update()
		{
			this.fsm.DoUpdate();
			this.managerPosition = this._manager.center.position;
		}

		public void ForceDestroy()
		{
			this.fsm.ChangeState(this.stDestroyed);
		}

		public void Regenerate()
		{
			this.hits = this.maxHits;
			this.fsm.ChangeState(this.stInactive);
		}

		public void Hide()
		{
			if (!this.fsm.IsInState(this.stDestroyed))
			{
				this.fsm.ChangeState(this.stHidden);
			}
		}

		public void Show()
		{
			if (!this.fsm.IsInState(this.stDestroyed))
			{
				this.fsm.ChangeState(this.stInactive);
			}
		}

		public void SetLaserParentRotation(Vector2 d)
		{
			base.transform.GetChild(0).right = d;
		}

		public void Init(BurntFaceRosaryManager manager)
		{
			this._manager = manager;
		}

		public bool IsDestroyed()
		{
			return this.fsm.IsInState(this.stDestroyed);
		}

		public void SetPattern(BurntFaceRosaryPattern pattern)
		{
			if (pattern.ID == "EMPTY")
			{
				if (!this.fsm.IsInState(this.stDestroyed))
				{
					this.fsm.ChangeState(this.stInactive);
				}
			}
			else if (!this.fsm.IsInState(this.stDestroyed))
			{
				this.fsm.ChangeState(this.stActive);
			}
			this._currentPattern = pattern;
			this.currentType = pattern.beadType;
			this.angleSections = pattern.activeSections;
			if (pattern.beadType == BurntFaceRosaryBead.ROSARY_BEAD_TYPE.PROJECTILE)
			{
				this._turret.SetFireParameters(pattern.projectileSpeed, pattern.projectileFireRate);
			}
			else
			{
				this._warningTime = pattern.beamWarningTime;
			}
		}

		public void UpdateAngle(float angleFromManager)
		{
			this.currentAngle = angleFromManager;
		}

		public void OnDestroyed()
		{
			this._manager.OnBeadDestroyed(this);
		}

		public bool IsInsideSection(BurntFaceRosaryAngles section)
		{
			return this.currentAngle < section.endAngle && this.currentAngle > section.startAngle;
		}

		public bool IsInsideActiveAngle()
		{
			if (this.angleSections == null || this.angleSections.Count == 0)
			{
				return false;
			}
			foreach (BurntFaceRosaryAngles section in this.angleSections)
			{
				if (this.IsInsideSection(section))
				{
					return true;
				}
			}
			return false;
		}

		public void ActivateTurret(bool activate)
		{
			this._turret.enabled = activate;
			this._turret.ForceShoot();
		}

		public void ActivateBeam()
		{
			this.beamLauncher.gameObject.SetActive(true);
			this.beamLauncher.ActivateDelayedBeam(this._warningTime, true);
			Debug.Log("ACTIVATING BEAM");
		}

		public void DeactivateBeam()
		{
			Debug.Log("DEACTIVATING BEAM");
			this.beamLauncher.ActivateBeamAnimation(false);
			base.StartCoroutine(this.DelayedBeamDeactivation(0.3f));
		}

		private IEnumerator DelayedBeamDeactivation(float seconds)
		{
			yield return new WaitForSeconds(seconds);
			this.beamLauncher.gameObject.SetActive(false);
			yield break;
		}

		public void ChangeToProjectile()
		{
			if (!this.fsm.IsInState(this.stDestroyed))
			{
				this.fsm.ChangeState(this.stProjectile);
			}
		}

		public void ChangeToBeam()
		{
			if (!this.fsm.IsInState(this.stDestroyed))
			{
				this.fsm.ChangeState(this.stBeam);
			}
		}

		public void UpdateChargeCounter()
		{
			this.chargeCounter += Time.deltaTime;
		}

		public bool IsCharged()
		{
			return this.currentType == BurntFaceRosaryBead.ROSARY_BEAD_TYPE.PROJECTILE || this.chargeCounter > 0.5f;
		}

		public void ResetChargeCounter()
		{
			this.chargeCounter = 0f;
		}

		public void ChangeToCharging()
		{
			this.fsm.ChangeState(this.stCharging);
		}

		public void ChangeToInactive()
		{
			this.fsm.ChangeState(this.stInactive);
		}

		public void ChangeToActive()
		{
			this.fsm.ChangeState(this.stActive);
		}

		public void SetAnimatorDestroyed(bool dest)
		{
		}

		public void PlayShowAnimation()
		{
		}

		public void PlayHideAnimation()
		{
		}

		public void SetColliderActive(bool v)
		{
		}

		public void OnDrawGizmosSelected()
		{
			if (this._manager == null)
			{
				return;
			}
			Gizmos.color = Color.magenta;
			if (this.angleSections != null)
			{
				foreach (BurntFaceRosaryAngles angle in this.angleSections)
				{
					this.DrawGizmoSection(angle);
				}
			}
		}

		private void DrawGizmoSection(BurntFaceRosaryAngles angle)
		{
			Quaternion quaternion = Quaternion.Euler(0f, 0f, angle.startAngle);
			Quaternion quaternion2 = Quaternion.Euler(0f, 0f, angle.endAngle);
			Vector2 right = Vector2.right;
			Vector2 vector = quaternion * right;
			Vector2 vector2 = quaternion2 * right;
			Gizmos.DrawSphere(this.managerPosition, 0.1f);
			Gizmos.color = Color.cyan;
			Gizmos.DrawRay(this.managerPosition, vector * 5f);
			Gizmos.color = Color.blue;
			Gizmos.DrawRay(this.managerPosition, vector2 * 5f);
			int num = Mathf.RoundToInt((angle.endAngle - angle.startAngle) / 2f);
			for (int i = 0; i < num; i++)
			{
				float num2 = (float)i / (float)num;
				Gizmos.color = Color.Lerp(Color.cyan, Color.blue, num2);
				Quaternion quaternion3 = Quaternion.Euler(0f, 0f, Mathf.Lerp(angle.startAngle, angle.endAngle, num2));
				Gizmos.DrawRay(this.managerPosition, quaternion3 * Vector2.right * 4f);
			}
			Vector2 vector3 = Quaternion.Euler(0f, 0f, this.currentAngle) * Vector2.right;
			Color color = Color.red;
			if (this.IsInsideActiveAngle())
			{
				color = Color.green;
			}
			Gizmos.color = color;
			Gizmos.DrawRay(this.managerPosition, vector3 * 5f);
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		private BurntFaceRosaryPattern _currentPattern;

		private BurntFaceRosaryManager _manager;

		public int revolutionsRemaining;

		public float currentAngle;

		public Vector2 managerPosition;

		public List<BurntFaceRosaryAngles> angleSections;

		public StateMachine<BurntFaceRosaryBead> fsm;

		public State<BurntFaceRosaryBead> stInactive;

		public State<BurntFaceRosaryBead> stActive;

		public State<BurntFaceRosaryBead> stHidden;

		public State<BurntFaceRosaryBead> stCharging;

		public State<BurntFaceRosaryBead> stBeam;

		public State<BurntFaceRosaryBead> stProjectile;

		public State<BurntFaceRosaryBead> stDestroyed;

		public TileableBeamLauncher beamLauncher;

		public BasicTurret _turret;

		public AttackArea beamAttackArea;

		public DamageEffectScript damageEffect;

		public int hits;

		public int maxHits;

		public bool canBeDamaged;

		public float _warningTime = 0.5f;

		public BurntFaceRosaryBead.ROSARY_BEAD_TYPE currentType;

		private const float MAX_CHARGE = 0.5f;

		private float chargeCounter;

		public enum ROSARY_BEAD_TYPE
		{
			BEAM,
			PROJECTILE
		}
	}
}
