using System;
using FMOD.Studio;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.BellGhost;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Projectiles;
using Sirenix.OdinInspector;
using Tools.Level;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.Traps.Turrets
{
	public class BasicTurret : MonoBehaviour, IProjectileAttack, IActionable
	{
		private void Start()
		{
			PoolManager.Instance.CreatePool(this.projectilePrefab.gameObject, 30);
			this._spriteRenderer = base.GetComponentInChildren<SpriteRenderer>();
			this._delayCounter = this.startDelay;
		}

		private void Update()
		{
			if (this.isActivated)
			{
				if (this._delayCounter > 0f)
				{
					this._delayCounter -= Time.deltaTime;
					if (this._delayCounter < 0f && this.shootOnceAfterDelay)
					{
						this._shootCounter = this.fireRate;
					}
				}
				else
				{
					this.CheckShoot();
				}
			}
		}

		private void CheckShoot()
		{
			this._shootCounter += Time.deltaTime;
			if (this._shootCounter > this.fireRate)
			{
				this._shootCounter = 0f;
				this.Shoot();
			}
		}

		public void ForceShoot()
		{
			this._shootCounter = this.fireRate + 1f;
		}

		protected virtual void Shoot()
		{
			if (this.shootUsingAnimation)
			{
				this.animator.SetTrigger("shoot");
				this.PlayShoot();
			}
			else
			{
				this.LaunchProjectile();
			}
		}

		public void SetFireParameters(float _speed, float _fireRate)
		{
			this.projectileSpeed = _speed;
			this.fireRate = _fireRate;
		}

		public void LaunchProjectile()
		{
			Vector2 v = this.GetDirectionFromEnum(this.direction);
			if (this.useTurretRightAsDirection)
			{
				v = base.transform.right;
			}
			StraightProjectile component = PoolManager.Instance.ReuseObject(this.projectilePrefab.gameObject, base.transform.position + this.projectileLaunchOffset, Quaternion.identity, false, 1).GameObject.GetComponent<StraightProjectile>();
			this.SetProjectileWeaponDamage(component, this.ProjectileDamageAmount);
			component.Init(v, this.projectileSpeed);
			component.timeToLive = this.destructionDistance / this.projectileSpeed;
			component.ResetTTL();
			if (this.onProjectileFired != null)
			{
				this.onProjectileFired(component);
			}
		}

		private Vector2 GetDirectionFromEnum(BasicTurret.TURRET_DIRECTION direction)
		{
			Vector2 result;
			switch (direction)
			{
			case BasicTurret.TURRET_DIRECTION.LEFT:
				result = Vector2.left;
				break;
			case BasicTurret.TURRET_DIRECTION.RIGHT:
				result = Vector2.right;
				break;
			case BasicTurret.TURRET_DIRECTION.UP:
				result = Vector2.up;
				break;
			case BasicTurret.TURRET_DIRECTION.DOWN:
				result = Vector2.down;
				break;
			default:
				result = Vector2.zero;
				break;
			}
			return result;
		}

		private void OnDrawGizmos()
		{
			if (!this.showGizmos)
			{
				return;
			}
			Gizmos.color = Color.red;
			Vector3 vector = base.transform.position + this.projectileLaunchOffset;
			float d = this.fireRate * this.projectileSpeed;
			Gizmos.DrawSphere(vector, 0.25f);
			Vector3 a = this.GetDirectionFromEnum(this.direction);
			Vector3 vector2 = vector + a * this.destructionDistance;
			for (int i = 1; i <= this.projectilesShown; i++)
			{
				Gizmos.DrawSphere(vector + a * d * (float)i, 0.1f);
			}
			Gizmos.color = Color.yellow;
			if (this.showLine)
			{
				Gizmos.DrawLine(vector, vector2);
			}
			Gizmos.DrawSphere(vector2, 0.25f);
		}

		public bool Locked
		{
			get
			{
				return this.isLocked;
			}
			set
			{
				this.isLocked = value;
			}
		}

		public void PlayShoot()
		{
			if (!this._spriteRenderer.IsVisibleFrom(Camera.main))
			{
				return;
			}
			if (this._shootAudioInstance.isValid())
			{
				this._shootAudioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			}
			this._shootAudioInstance = Core.Audio.CreateEvent(this.shootSound, default(Vector3));
			this._shootAudioInstance.start();
		}

		public void SetProjectileWeaponDamage(int damage)
		{
			if (damage > 0)
			{
				this.ProjectileDamageAmount = damage;
			}
		}

		public void SetProjectileWeaponDamage(Projectile projectile, int damage)
		{
			this.SetProjectileWeaponDamage(damage);
			if (damage <= 0 || projectile == null)
			{
				return;
			}
			ProjectileWeapon componentInChildren = projectile.GetComponentInChildren<ProjectileWeapon>();
			if (componentInChildren)
			{
				componentInChildren.SetDamage(damage);
			}
		}

		public void Use()
		{
			this.isActivated = !this.isActivated;
		}

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		public bool isActivated = true;

		[BoxGroup("Design Settings", true, false, 0)]
		public int ProjectileDamageAmount;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		[Tooltip("Seconds between each shot")]
		[SuffixLabel("seconds", false)]
		private float fireRate = 0.5f;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private BasicTurret.TURRET_DIRECTION direction = BasicTurret.TURRET_DIRECTION.RIGHT;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private StraightProjectile projectilePrefab;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private Vector2 projectileLaunchOffset;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private float projectileSpeed;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private float destructionDistance;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		[Tooltip("Initial delay before firing the first bullet")]
		[SuffixLabel("seconds", false)]
		private float startDelay;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		[Tooltip("If true the first shot comes right after the delay. If not, it will also wait its fireRate before the first shot is fired")]
		private bool shootOnceAfterDelay;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private int bulletPoolSize;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		[Tooltip("If checked, the turret will fire towards it's local right (x) vector, shown as red in Unity")]
		private bool useTurretRightAsDirection;

		public bool shootUsingAnimation = true;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		private string shootSound;

		[SerializeField]
		[FoldoutGroup("Debug settings", 0)]
		private bool showGizmos = true;

		[SerializeField]
		[FoldoutGroup("Debug settings", 0)]
		private bool showLine = true;

		[SerializeField]
		[FoldoutGroup("Debug settings", 0)]
		private int projectilesShown = 3;

		public Animator animator;

		private SpriteRenderer _spriteRenderer;

		private float _shootCounter;

		private float _delayCounter;

		public Action<Projectile> onProjectileFired;

		private EventInstance _shootAudioInstance;

		private bool isLocked;

		public enum TURRET_DIRECTION
		{
			LEFT,
			RIGHT,
			UP,
			DOWN
		}
	}
}
