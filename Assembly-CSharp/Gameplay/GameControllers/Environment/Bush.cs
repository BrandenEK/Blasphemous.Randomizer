using System;
using UnityEngine;

namespace Gameplay.GameControllers.Environment
{
	public class Bush : Vegetation
	{
		private void Awake()
		{
			this.plantAnimator = base.GetComponent<Animator>();
			this.plantCollider = base.GetComponent<BoxCollider2D>();
		}

		private void Start()
		{
			this.plantAnimator.speed = 0f;
		}

		private void Update()
		{
			if (this.isShaking)
			{
				this.Shaking(this.timeShakingEntityPassing);
			}
		}

		public override void Shaking(float timeShaking)
		{
			this.deltaPlayTime += Time.deltaTime;
			if (this.deltaPlayTime <= timeShaking)
			{
				this.plantAnimator.speed = 1f;
			}
			else
			{
				this.isShaking = !this.isShaking;
				this.deltaPlayTime = 0f;
				this.plantAnimator.speed = 0f;
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject.layer == LayerMask.NameToLayer("Penitent") && !this.isShaking)
			{
				this.isShaking = true;
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (other.gameObject.layer == LayerMask.NameToLayer("Penitent") && !this.isShaking)
			{
				this.isShaking = true;
			}
		}

		public float timeShakingEntityPassing;

		private float deltaPlayTime;
	}
}
