using System;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.Traps.SpikesTrap
{
	public class SpikeTrap : MonoBehaviour
	{
		public bool IsVisible
		{
			get
			{
				return this.isVisible;
			}
		}

		public bool SpikeAreRaised
		{
			get
			{
				return this.spikesAreRaised;
			}
		}

		public BoxCollider2D SpikeTrapCollider
		{
			get
			{
				return this.spikeTrapColllider;
			}
		}

		private void Update()
		{
			bool flag = this.IsVisibleFrom(this.spikeRenderer, Camera.main);
			if (this.isVisible != flag)
			{
				this.isVisible = flag;
			}
		}

		private void Awake()
		{
			this.deathAnimator = base.GetComponent<Animator>();
			this.spikeTrapColllider = base.GetComponent<BoxCollider2D>();
			this.spikeRenderer = base.GetComponent<SpriteRenderer>();
		}

		public void RiseSpikes(bool rise = true)
		{
			if (rise && !this.spikesAreRaised)
			{
				this.spikesAreRaised = true;
				this.deathAnimator.SetBool("RISE", rise);
			}
			else if (!rise && this.spikesAreRaised)
			{
				this.spikesAreRaised = !this.spikesAreRaised;
				this.deathAnimator.SetBool("RISE", this.spikesAreRaised);
			}
		}

		public bool IsVisibleFrom(Renderer renderer, Camera camera)
		{
			Plane[] array = GeometryUtility.CalculateFrustumPlanes(camera);
			return GeometryUtility.TestPlanesAABB(array, renderer.bounds);
		}

		private Animator deathAnimator;

		private bool spikesAreRaised;

		private BoxCollider2D spikeTrapColllider;

		private bool isVisible;

		private SpriteRenderer spikeRenderer;
	}
}
