using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Environment;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.NPCs.BloodDecals
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(SpriteRenderer))]
	public class BloodDecal : MonoBehaviour
	{
		public PermaBlood PermaBlood
		{
			get
			{
				return this.permaBlood;
			}
			set
			{
				this.permaBlood = value;
			}
		}

		private void Awake()
		{
			this.bloodDecalAnimator = base.GetComponent<Animator>();
			this.bloodDecalSprite = base.GetComponent<SpriteRenderer>();
		}

		public void OnEnable()
		{
			this.deltaBloodDecalFreezeTime = 0f;
			if (this.bloodDecalAnimator != null)
			{
				this.bloodDecalAnimator.speed = 0f;
			}
		}

		private void Start()
		{
			this.bloodDecalPumper = base.GetComponentInParent<BloodDecalPumper>();
			this.levelEffectStore = Core.Logic.CurrentLevelConfig.LevelEffectsStore;
		}

		private void Update()
		{
			this.deltaBloodDecalFreezeTime += Time.deltaTime;
			if (this.deltaBloodDecalFreezeTime >= this.bloodDecalFreezeTime)
			{
				this.bloodDecalAnimator.speed = 1f;
			}
		}

		public void Dispose()
		{
			if (this.bloodDecalPumper != null)
			{
				this.LeavePermaBlood();
				this.bloodDecalPumper.DisposeBloodDecal(this);
			}
		}

		public void PlayBloodDecalAnimation()
		{
			if (this.bloodDecalAnimator != null)
			{
				this.bloodDecalAnimator.Play("PumpBlood");
			}
		}

		public void LeavePermaBlood()
		{
			if (this.permaBlood != null)
			{
				SpawnPoint permaBloodSpawnPoint = this.bloodDecalPumper.PermaBloodSpawnPoint;
				GameObject gameObject = this.permaBlood.Instance(permaBloodSpawnPoint.transform.position, base.transform.rotation);
				gameObject.transform.parent = this.levelEffectStore.transform;
			}
		}

		public void SetOrientation(EntityOrientation orientation)
		{
			this.bloodDecalSprite.flipX = (orientation == EntityOrientation.Left);
		}

		private Animator bloodDecalAnimator;

		private SpriteRenderer bloodDecalSprite;

		private BloodDecalPumper bloodDecalPumper;

		[SerializeField]
		private float bloodDecalFreezeTime = 0.1f;

		private float deltaBloodDecalFreezeTime;

		public bool hasPermaBlood;

		[SerializeField]
		protected PermaBlood permaBlood;

		private LevelEffectsStore levelEffectStore;
	}
}
