using System;
using DG.Tweening;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Familiar.AI
{
	public class FamiliarBehaviour : MonoBehaviour
	{
		public Familiar Familiar { get; private set; }

		public Enemy CherubInstance { get; private set; }

		public Vector3 ChaseVelocity
		{
			get
			{
				return this._velocity;
			}
		}

		private void Awake()
		{
			this.Familiar = base.GetComponent<Familiar>();
		}

		private void Start()
		{
			DOTween.To(delegate(float x)
			{
				this._currentAmplitudeX = x;
			}, this._currentAmplitudeX, this.AmplitudeX, 1f);
			DOTween.To(delegate(float x)
			{
				this._currentAmplitudeY = x;
			}, this._currentAmplitudeY, this.AmplitudeY, 1f);
			this.Familiar.StateMachine.SwitchState<FamiliarChasePlayerState>();
			base.Invoke("LookForCherub", 3f);
		}

		public void ChasingEntity(Entity entity, Vector2 offset)
		{
			Vector3 chasingTargetPosition = this.GetChasingTargetPosition(entity, offset);
			chasingTargetPosition.y = entity.transform.position.y + offset.y;
			this.Familiar.transform.position = Vector3.SmoothDamp(this.Familiar.transform.position, chasingTargetPosition, ref this._velocity, this.ChasingElongation, this.ChasingSpeed);
		}

		private Vector3 GetChasingTargetPosition(Entity entity, Vector2 offset)
		{
			Vector3 position = entity.transform.position;
			if (entity.Status.Orientation == EntityOrientation.Left)
			{
				position.x += offset.x;
			}
			else
			{
				position.x -= offset.x;
			}
			return position;
		}

		public void SetOrientationByVelocity(Vector3 velocity)
		{
			if (velocity.x < 0f)
			{
				this.Familiar.SetOrientation(EntityOrientation.Left, true, false);
			}
			else if (velocity.x > 0f)
			{
				this.Familiar.SetOrientation(EntityOrientation.Right, true, false);
			}
		}

		public void Floating()
		{
			this._index += Time.deltaTime;
			float x = this._currentAmplitudeX * Mathf.Sin(this.SpeedX * this._index);
			float y = Mathf.Cos(this.SpeedY * this._index) * this._currentAmplitudeY;
			this.Familiar.SpriteRenderer.transform.localPosition = new Vector3(x, y, 0f);
		}

		private void LookForCherub()
		{
			if (this.FindCherub())
			{
				this.PursueCherub();
			}
		}

		private bool FindCherub()
		{
			Enemy[] array = UnityEngine.Object.FindObjectsOfType<Enemy>();
			bool result = false;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].name.Contains("CollectibleCherubCaptor"))
				{
					Debug.Log("cherub name: " + array[i].name);
					result = true;
					this.CherubInstance = array[i];
					this.CherubInstance.OnDeath += this.OnCherubRescued;
					break;
				}
			}
			return result;
		}

		private void PursueCherub()
		{
			this.Familiar.StateMachine.SwitchState<FamiliarChaseCherubState>();
		}

		private void OnCherubRescued()
		{
			this.CherubInstance.OnDeath -= this.OnCherubRescued;
			this.CherubInstance = null;
			this.Familiar.StateMachine.SwitchState<FamiliarChasePlayerState>();
		}

		private void OnDestroy()
		{
			if (this.CherubInstance != null && !this.CherubInstance.Status.Dead)
			{
				this.CherubInstance.OnDeath -= this.OnCherubRescued;
			}
		}

		public Enemy Target;

		public Vector2 PlayerOffsetPosition;

		public Vector2 CherubOffsetPosition;

		[FoldoutGroup("Floating Motion", true, 0)]
		public float AmplitudeX = 10f;

		[FoldoutGroup("Floating Motion", true, 0)]
		public float AmplitudeY = 5f;

		[FoldoutGroup("Floating Motion", true, 0)]
		public float SpeedX = 1f;

		[FoldoutGroup("Floating Motion", true, 0)]
		public float SpeedY = 2f;

		[FoldoutGroup("Chasing Options", true, 0)]
		public float CherubCriticalDistance = 4f;

		public const string CherubName = "CollectibleCherubCaptor";

		[FoldoutGroup("Chasing", true, 0)]
		public float ChasingElongation = 0.5f;

		[FoldoutGroup("Chasing", true, 0)]
		public float ChasingSpeed = 5f;

		private Vector3 _velocity = Vector3.zero;

		private float _index;

		private float _currentAmplitudeY;

		private float _currentAmplitudeX;
	}
}
