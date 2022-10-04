using System;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Audio
{
	[RequireComponent(typeof(CircleCollider2D))]
	public class AreaModifier : AudioTool
	{
		protected override void BaseStart()
		{
			base.IsEmitter = false;
			this.col = base.GetComponent<CircleCollider2D>();
			if (this.sensor != null)
			{
				this.sensor.OnPenitentEnter += this.OnPenitentEnter;
				this.sensor.OnPenitentExit += this.OnPenitentExit;
			}
		}

		protected override void BaseDestroy()
		{
			if (this.sensor != null)
			{
				this.sensor.OnPenitentEnter -= this.OnPenitentEnter;
				this.sensor.OnPenitentExit -= this.OnPenitentExit;
			}
		}

		private void OnPenitentEnter()
		{
			this.insideActivationRange = true;
		}

		private void OnPenitentExit()
		{
			this.insideActivationRange = false;
		}

		protected override void BaseUpdate()
		{
			if (this.InsideInfluenceArea || !this.allParamsAtZero)
			{
				this.UpdateParameters();
			}
		}

		protected override void BaseDrawGizmos()
		{
			Gizmos.color = Color.gray;
			Gizmos.DrawWireSphere(base.transform.position, this.OriginSize);
		}

		private void UpdateParameters()
		{
			bool flag = true;
			float value = (!this.insideActivationRange) ? 0f : this.CalculateParameterValue(this.TargetDistance, this.InfluenceDistance);
			Core.Audio.Ambient.AddAreaModifier(base.name, value);
			for (int i = 0; i < this.parameters.Length; i++)
			{
				string name = this.parameters[i].name;
				this.StepParamValue(name, value);
				this.parameters[i].currentValue = Core.Audio.Ambient.GetParameterValue(name);
				if (this.parameters[i].currentValue > 0f)
				{
					flag = false;
				}
			}
			this.allParamsAtZero = flag;
		}

		private void StepParamValue(string param, float value)
		{
			float parameterValue = Core.Audio.Ambient.GetParameterValue(param);
			if (Mathf.Approximately(parameterValue, value))
			{
				Core.Audio.Ambient.SetParameter(param, value);
			}
			else if (parameterValue > value)
			{
				Core.Audio.Ambient.SetParameter(param, parameterValue - Time.deltaTime * this.transitionSpeed);
			}
			else if (parameterValue < value)
			{
				Core.Audio.Ambient.SetParameter(param, parameterValue + Time.deltaTime * this.transitionSpeed);
			}
		}

		private float CalculateParameterValue(float originDistance, float influenceArea)
		{
			float num = Math.Max(0f, originDistance - this.OriginSize);
			float num2 = influenceArea - this.OriginSize;
			return this.maxParamValue * (1f - num / num2);
		}

		private bool InsideInfluenceArea
		{
			get
			{
				return this.TargetDistance <= this.InfluenceDistance;
			}
		}

		private float TargetDistance
		{
			get
			{
				if (!Core.ready || Core.Logic.Penitent == null)
				{
					return -1f;
				}
				Vector2 a = Core.Logic.Penitent.transform.position;
				return Vector2.Distance(a, this.OriginPosition);
			}
		}

		private Vector2 OriginPosition
		{
			get
			{
				return base.transform.position;
			}
		}

		private float OriginSize
		{
			get
			{
				if (this.col == null)
				{
					return 0f;
				}
				return this.col.radius * this.originSize;
			}
		}

		private float InfluenceDistance
		{
			get
			{
				if (this.col == null)
				{
					return 0f;
				}
				return this.col.radius;
			}
		}

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[Range(0f, 1f)]
		private float maxParamValue = 1f;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[Range(0f, 1f)]
		private float originSize = 0.5f;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[Range(0f, 2f)]
		private float transitionSpeed = 1f;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		private CollisionSensor sensor;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		private AudioParamName[] parameters = new AudioParamName[0];

		private CircleCollider2D col;

		private bool allParamsAtZero;

		private bool insideActivationRange;
	}
}
