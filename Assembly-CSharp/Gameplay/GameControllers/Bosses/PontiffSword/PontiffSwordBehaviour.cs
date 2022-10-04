using System;
using System.Diagnostics;
using DG.Tweening;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Bosses.PontiffGiant;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using Tools.Level.Actionables;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PontiffSword
{
	public class PontiffSwordBehaviour : EnemyBehaviour
	{
		public PontiffSword PontiffSword { get; set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnSwordDestroyed;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnPlungeFinished;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnSlashFinished;

		public override void OnAwake()
		{
			base.OnAwake();
			this.groundHits = new RaycastHit2D[1];
			this.PontiffSword = base.GetComponent<PontiffSword>();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.currentSwordState == SWORD_STATES.FLYING_AROUND)
			{
				this.UpdateHeightOffset();
				this.UpdateFlyingAround();
			}
		}

		private void UpdateHeightOffset()
		{
			if (Core.Logic.Penitent == null)
			{
				return;
			}
			if (this.GetBaseHeight() < Core.Logic.Penitent.transform.position.y)
			{
				this._heightOffset += Time.deltaTime * 1f;
			}
			else
			{
				this._heightOffset -= Time.deltaTime * 1f;
			}
		}

		public void ChangeSwordState(SWORD_STATES st)
		{
			this.currentSwordState = st;
			if (st == SWORD_STATES.FLYING_AROUND)
			{
				this._flyAroundCounter = 0f;
			}
		}

		private Vector2 GetDirToPenitent(Vector3 from)
		{
			return Core.Logic.Penitent.transform.position - from;
		}

		private void UpdateFlyingAround()
		{
			Vector2 b = new Vector2(-Mathf.Sign((Core.Logic.Penitent.transform.position - base.transform.position).x) * this.chasingOffset.x, this.chasingOffset.y);
			Vector2 vector = Core.Logic.Penitent.transform.position + b;
			this._flyAroundCounter += Time.deltaTime;
			Vector2 normalized = (vector - base.transform.position).normalized;
			GameplayUtils.DrawDebugCross(vector, Color.magenta, 0.1f);
			float z = Mathf.Sin(this._flyAroundCounter * this.rotatingFreq) * this.maxAngle;
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(0f, 0f, z), this.rotationDampFactor);
			float d = (float)((Mathf.Sign(this.velocity.x) != Mathf.Sign(normalized.x)) ? 2 : 1);
			float num = Vector2.Distance(base.transform.position, vector);
			float num2 = 2f;
			if (num < num2)
			{
				this.velocity -= this.velocity * 0.5f * Time.deltaTime;
			}
			else
			{
				this.velocity += normalized.normalized * this.accel * Time.deltaTime * d;
			}
			this.velocity = Vector2.ClampMagnitude(this.velocity, this.maxSpeed);
			Vector3 b2 = this.velocity * Time.deltaTime;
			base.transform.position += b2;
		}

		private bool IsPointOutsideBattleBoundaries(Vector2 p)
		{
			float num = Mathf.Abs(p.x - this.bossfightPoints.fightCenterTransform.position.x);
			return num > 4.6f;
		}

		public void ActivateTrails(bool activate)
		{
			this.parentOfTrails.SetActive(activate);
		}

		public void Slash()
		{
			float postSlashAngle = 360f + -1f * this.preSlashAngle;
			float sign = Mathf.Sign(this.GetDirToPenitent(base.transform.position).x);
			base.transform.DOKill(false);
			this.currentXTween = base.transform.DOMoveX(base.transform.position.x + this.slashXMovement * sign, this.anticipationDuration + this.attackDuration * 0.5f, false).SetEase(Ease.InBack).OnUpdate(new TweenCallback(this.CheckCollision));
			base.transform.DOMoveY(base.transform.position.y + this.slashYMovement, this.anticipationDuration + this.attackDuration * 0.25f, false).SetEase(Ease.InOutQuad).SetLoops(2, LoopType.Yoyo);
			base.transform.DORotate(new Vector3(0f, 0f, this.preSlashAngle * sign), this.anticipationDuration, RotateMode.LocalAxisAdd).SetEase(this.anticipationEasing).OnComplete(delegate
			{
				this.PontiffSword.Audio.PlaySlash_AUDIO();
				this.ActivateTrails(true);
				this.meleeAttack.damageOnEnterArea = true;
				this.meleeAttack.OnMeleeAttackGuarded += this.OnMeleeAttackGuarded;
				this.transform.DORotate(new Vector3(0f, 0f, postSlashAngle * sign), this.attackDuration, RotateMode.LocalAxisAdd).SetEase(this.attackEasing).OnComplete(delegate
				{
					this.OnMeleeAttackFinished();
				});
			});
		}

		private void CheckCollision()
		{
			if (this.motionChecker.HitsBlock)
			{
				this.currentXTween.Kill(true);
			}
		}

		private void OnMeleeAttackGuarded()
		{
			this.meleeAttack.OnMeleeAttackGuarded -= this.OnMeleeAttackGuarded;
			base.transform.DOKill(false);
			float num = Mathf.Sign(this.GetDirToPenitent(base.transform.position).x);
			base.transform.DOMoveX(base.transform.position.x - this.slashXMovement * num, this.anticipationDuration + this.attackDuration * 0.5f, false).SetEase(Ease.InBack);
			base.transform.DORotate(new Vector3(0f, 0f, -90f * num), 0.3f, RotateMode.LocalAxisAdd).SetEase(Ease.OutBack).OnComplete(delegate
			{
				this.OnMeleeAttackFinished();
			});
		}

		private void Repullo()
		{
			base.transform.DOKill(false);
			Vector2 dirToPenitent = this.GetDirToPenitent(base.transform.position);
			this.ChangeSwordState(SWORD_STATES.STUN);
			this.velocity = Vector2.zero;
			base.transform.DOMove(base.transform.position - dirToPenitent.normalized * 3f, 0.4f, false).SetEase(Ease.OutQuad).OnComplete(delegate
			{
				this.ChangeSwordState(SWORD_STATES.FLYING_AROUND);
			});
		}

		private void OnMeleeAttackFinished()
		{
			this.meleeAttack.OnMeleeAttackGuarded -= this.OnMeleeAttackGuarded;
			this.meleeAttack.damageOnEnterArea = false;
			this.ActivateTrails(false);
			if (this.OnSlashFinished != null)
			{
				this.OnSlashFinished();
			}
		}

		public void Plunge()
		{
			base.transform.DOKill(false);
			base.transform.DORotate(Vector3.zero, 0.5f, RotateMode.Fast);
			base.transform.DOMoveY(base.transform.position.y + 3f, this.anticipationDuration, false).SetEase(Ease.InOutQuad).OnComplete(delegate
			{
				this.meleeAttack.damageOnEnterArea = true;
				this.PontiffSword.Audio.PlayPlunge_AUDIO();
				Physics2D.Raycast(base.transform.position, Vector2.down, this.filter, this.groundHits, 30f);
				float num = this.groundHits[0].distance - 1.5f;
				UnityEngine.Debug.DrawLine(base.transform.position, this.groundHits[0].point, Color.green, 5f);
				this.ActivateTrails(true);
				base.transform.DOMoveY(base.transform.position.y - num, this.plungeDuration, false).SetEase(Ease.InExpo).OnComplete(delegate
				{
					this.PlungeFinished();
				});
			});
		}

		private void PlungeFinished()
		{
			this.ActivateTrails(false);
			this.meleeAttack.damageOnEnterArea = false;
			if (this.OnPlungeFinished != null)
			{
				this.OnPlungeFinished();
			}
		}

		private float GetBaseHeight()
		{
			return this.bossfightPoints.fightCenterTransform.position.y + this.normalHeight;
		}

		public void BackToFlyingAround()
		{
			base.transform.DOKill(false);
			float baseHeight = this.GetBaseHeight();
			base.transform.DOMoveY(baseHeight, 2.5f, false).SetEase(Ease.InQuint).OnComplete(delegate
			{
				this.ChangeSwordState(SWORD_STATES.FLYING_AROUND);
			});
			base.transform.DORotate(Vector3.zero, 0.5f, RotateMode.Fast);
		}

		public void Move(Vector2 pos, float duration = 0.5f, TweenCallback callback = null)
		{
			base.transform.DOMove(pos, duration, false).SetEase(Ease.InOutQuad).onComplete = callback;
		}

		public override void Attack()
		{
			throw new NotImplementedException();
		}

		public override void Chase(Transform targetPosition)
		{
			throw new NotImplementedException();
		}

		public void Revive()
		{
			this.PontiffSword.animatorInyector.Alive(true);
			base.Invoke("BackToFlyingAround", 1f);
			this.eyeTransform.gameObject.SetActive(true);
		}

		public void Death()
		{
			if (this.OnSwordDestroyed != null)
			{
				this.OnSwordDestroyed();
			}
			this.ChangeSwordState(SWORD_STATES.DESTROYED);
			this.eyeTransform.gameObject.SetActive(false);
			this.meleeAttack.OnMeleeAttackGuarded -= this.OnMeleeAttackGuarded;
			this.meleeAttack.damageOnEnterArea = false;
			this.ActivateTrails(false);
			base.transform.DOKill(false);
			base.transform.DORotate(Vector3.zero, 0.3f, RotateMode.Fast).OnComplete(delegate
			{
				this.PontiffSword.animatorInyector.Alive(false);
			});
			base.Invoke("ActivatePlatform", 1f);
		}

		public void ActivatePlatform()
		{
		}

		public override void Damage()
		{
			if (this.currentSwordState == SWORD_STATES.FLYING_AROUND)
			{
				this.Repullo();
			}
		}

		public override void Idle()
		{
		}

		public override void StopMovement()
		{
		}

		public override void Wander()
		{
		}

		public GameObject parentOfTrails;

		private AshPlatform lastOne;

		public float preSlashAngle = -120f;

		public float anticipationDuration = 1.5f;

		public Ease anticipationEasing = Ease.OutQuad;

		public float attackDuration = 1.4f;

		public Ease attackEasing = Ease.InOutQuad;

		public float plungeDuration = 0.25f;

		public float plungeDistance = -6f;

		private float slashXMovement = 8f;

		private float slashYMovement = -3f;

		private float _heightOffset;

		public ContactFilter2D filter;

		public RaycastHit2D[] groundHits;

		public SWORD_STATES currentSwordState;

		public PontiffSwordMeleeAttack meleeAttack;

		public PontiffGiantBossfightPoints bossfightPoints;

		public Transform eyeTransform;

		public EntityMotionChecker motionChecker;

		[FoldoutGroup("Floating settings", 0)]
		public float speedFactor = 0.1f;

		[FoldoutGroup("Floating settings", 0)]
		public float floatingFreq = 0.1f;

		[FoldoutGroup("Floating settings", 0)]
		public float rotatingFreq = 2.75f;

		[FoldoutGroup("Floating settings", 0)]
		public float floatingAmp = 1f;

		[FoldoutGroup("Floating settings", 0)]
		public float normalHeight = 4f;

		[FoldoutGroup("Floating settings", 0)]
		public float maxAngle = 10f;

		[FoldoutGroup("Floating settings", 0)]
		public float rotationDampFactor = 0.05f;

		[FoldoutGroup("Floating settings", 0)]
		public float yFloatingAccel = 0.5f;

		[FoldoutGroup("Floating new settings", 0)]
		public float accel = 0.5f;

		[FoldoutGroup("Floating new settings", 0)]
		public float maxSpeed = 0.5f;

		[FoldoutGroup("Floating new settings", 0)]
		public Vector2 velocity;

		[FoldoutGroup("Floating new settings", 0)]
		public Vector2 chasingOffset;

		private Tween currentXTween;

		private float _flyAroundCounter;
	}
}
