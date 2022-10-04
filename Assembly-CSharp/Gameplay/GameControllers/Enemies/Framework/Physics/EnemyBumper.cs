using System;
using System.Collections;
using CreativeSpore.SmartColliders;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Framework.Physics
{
	public class EnemyBumper : MonoBehaviour
	{
		public bool EnemyBumperIsActive
		{
			get
			{
				return this.enemyBumperIsActive;
			}
			set
			{
				this.enemyBumperIsActive = value;
			}
		}

		private void Awake()
		{
			this.widthCollider = this._enemyBoxCollider.bounds.extents.x;
			this.bumperEnemy = false;
		}

		private void Start()
		{
			this._penitent = Core.Logic.Penitent;
			this.enemy = base.GetComponentInParent<Enemy>();
			this._enemyController = this.enemy.GetComponentInChildren<PlatformCharacterController>();
			this.enemyLayerValue = LayerMask.NameToLayer("Enemy");
			this.defaultLayerValue = LayerMask.NameToLayer("Penitent");
			this.enemyBumperIsActive = true;
		}

		private void Update()
		{
			if (this._enemyController.IsGrounded)
			{
				this.IncludeEnemyLayer(true);
			}
			else
			{
				this.IncludeEnemyLayer(false);
			}
		}

		private void OnTriggerStay2D(Collider2D collision)
		{
			if (this._penitent == null)
			{
				return;
			}
			if ((this.enemyLayer.value & 1 << collision.gameObject.layer) <= 0 || !this.enemy.EnemyFloorChecker().IsGrounded || !this.enemyBumperIsActive)
			{
				return;
			}
			if (!this._penitent.Status.IsGrounded || this._penitent.Status.Dead || this._penitent.IsDashing)
			{
				return;
			}
			float magnitude = (this._penitent.transform.position - base.transform.position).magnitude;
			float num = collision.bounds.extents.x + this.widthCollider;
			if (magnitude / 1.25f > num || this.bumperEnemy)
			{
				return;
			}
			this.bumperEnemy = true;
			this.bumperCoroutine = this.EnemyBumperCoroutine(num);
			base.StartCoroutine(this.bumperCoroutine);
		}

		private IEnumerator EnemyBumperCoroutine(float playerWidthCollider)
		{
			if (this._penitent == null)
			{
				yield return null;
			}
			float distance = playerWidthCollider + this.widthCollider;
			Vector2 dir = (this._penitent.transform.position.x < base.transform.position.x) ? Vector2.right : (-Vector2.right);
			while ((this._penitent.transform.position - base.transform.position).magnitude <= distance * 1.25f && this._penitent.Status.IsGrounded)
			{
				base.transform.parent.Translate(dir * this.bumperForce * Time.deltaTime, Space.World);
				yield return new WaitForEndOfFrame();
			}
			this.bumperEnemy = false;
			yield break;
		}

		public void IncludeEnemyLayer(bool include = true)
		{
			if (include && !this.includeEnemyLayer)
			{
				this.includeEnemyLayer = true;
				base.gameObject.layer = this.enemyLayerValue;
			}
			else if (!include && this.includeEnemyLayer)
			{
				this.includeEnemyLayer = false;
				base.gameObject.layer = this.defaultLayerValue;
			}
		}

		[SerializeField]
		private BoxCollider2D _enemyBoxCollider;

		private PlatformCharacterController _enemyController;

		private Penitent _penitent;

		private IEnumerator bumperCoroutine;

		private bool bumperEnemy;

		[Tooltip("The force applied to the enemy bumper displacement")]
		[Range(0f, 5f)]
		public float bumperForce;

		private int defaultLayerValue;

		private Enemy enemy;

		public bool enemyBumperIsActive;

		public LayerMask enemyLayer;

		private int enemyLayerValue;

		private bool includeEnemyLayer;

		private float widthCollider;
	}
}
