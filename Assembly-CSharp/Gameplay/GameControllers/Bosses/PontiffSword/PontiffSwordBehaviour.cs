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
			Vector2 vector;
			vector..ctor(-Mathf.Sign((Core.Logic.Penitent.transform.position - base.transform.position).x) * this.chasingOffset.x, this.chasingOffset.y);
			Vector2 vector2 = Core.Logic.Penitent.transform.position + vector;
			this._flyAroundCounter += Time.deltaTime;
			Vector2 normalized = (vector2 - base.transform.position).normalized;
			GameplayUtils.DrawDebugCross(vector2, Color.magenta, 0.1f);
			float num = Mathf.Sin(this._flyAroundCounter * this.rotatingFreq) * this.maxAngle;
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(0f, 0f, num), this.rotationDampFactor);
			float num2 = (float)((Mathf.Sign(this.velocity.x) != Mathf.Sign(normalized.x)) ? 2 : 1);
			float num3 = Vector2.Distance(base.transform.position, vector2);
			float num4 = 2f;
			if (num3 < num4)
			{
				this.velocity -= this.velocity * 0.5f * Time.deltaTime;
			}
			else
			{
				this.velocity += normalized.normalized * this.accel * Time.deltaTime * num2;
			}
			this.velocity = Vector2.ClampMagnitude(this.velocity, this.maxSpeed);
			Vector3 vector3 = this.velocity * Time.deltaTime;
			base.transform.position += vector3;
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
			ShortcutExtensions.DOKill(base.transform, false);
			this.currentXTween = TweenSettingsExtensions.OnUpdate<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveX(base.transform, base.transform.position.x + this.slashXMovement * sign, this.anticipationDuration + this.attackDuration * 0.5f, false), 26), new TweenCallback(this.CheckCollision));
			TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(base.transform, base.transform.position.y + this.slashYMovement, this.anticipationDuration + this.attackDuration * 0.25f, false), 7), 2, 1);
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(base.transform, new Vector3(0f, 0f, this.preSlashAngle * sign), this.anticipationDuration, 3), this.anticipationEasing), delegate()
			{
				this.PontiffSword.Audio.PlaySlash_AUDIO();
				this.ActivateTrails(true);
				this.meleeAttack.damageOnEnterArea = true;
				this.meleeAttack.OnMeleeAttackGuarded += this.OnMeleeAttackGuarded;
				TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(this.transform, new Vector3(0f, 0f, postSlashAngle * sign), this.attackDuration, 3), this.attackEasing), delegate()
				{
					this.OnMeleeAttackFinished();
				});
			});
		}

		private void CheckCollision()
		{
			if (this.motionChecker.HitsBlock)
			{
				TweenExtensions.Kill(this.currentXTween, true);
			}
		}

		private void OnMeleeAttackGuarded()
		{
			this.meleeAttack.OnMeleeAttackGuarded -= this.OnMeleeAttackGuarded;
			ShortcutExtensions.DOKill(base.transform, false);
			float num = Mathf.Sign(this.GetDirToPenitent(base.transform.position).x);
			TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveX(base.transform, base.transform.position.x - this.slashXMovement * num, this.anticipationDuration + this.attackDuration * 0.5f, false), 26);
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(base.transform, new Vector3(0f, 0f, -90f * num), 0.3f, 3), 27), delegate()
			{
				this.OnMeleeAttackFinished();
			});
		}

		private void Repullo()
		{
			ShortcutExtensions.DOKill(base.transform, false);
			Vector2 dirToPenitent = this.GetDirToPenitent(base.transform.position);
			this.ChangeSwordState(SWORD_STATES.STUN);
			this.velocity = Vector2.zero;
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(base.transform, base.transform.position - dirToPenitent.normalized * 3f, 0.4f, false), 6), delegate()
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
			ShortcutExtensions.DOKill(base.transform, false);
			ShortcutExtensions.DORotate(base.transform, Vector3.zero, 0.5f, 0);
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(base.transform, base.transform.position.y + 3f, this.anticipationDuration, false), 7), delegate()
			{
				this.meleeAttack.damageOnEnterArea = true;
				this.PontiffSword.Audio.PlayPlunge_AUDIO();
				Physics2D.Raycast(base.transform.position, Vector2.down, this.filter, this.groundHits, 30f);
				float num = this.groundHits[0].distance - 1.5f;
				Debug.DrawLine(base.transform.position, this.groundHits[0].point, Color.green, 5f);
				this.ActivateTrails(true);
				TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(base.transform, base.transform.position.y - num, this.plungeDuration, false), 17), delegate()
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
			ShortcutExtensions.DOKill(base.transform, false);
			float baseHeight = this.GetBaseHeight();
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(base.transform, baseHeight, 2.5f, false), 14), delegate()
			{
				this.ChangeSwordState(SWORD_STATES.FLYING_AROUND);
			});
			ShortcutExtensions.DORotate(base.transform, Vector3.zero, 0.5f, 0);
		}

		public void Move(Vector2 pos, float duration = 0.5f, TweenCallback callback = null)
		{
			TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(base.transform, pos, duration, false), 7).onComplete = callback;
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
			ShortcutExtensions.DOKill(base.transform, false);
			TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DORotate(base.transform, Vector3.zero, 0.3f, 0), delegate()
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

		public Ease anticipationEasing = 6;

		public float attackDuration = 1.4f;

		public Ease attackEasing = 7;

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
