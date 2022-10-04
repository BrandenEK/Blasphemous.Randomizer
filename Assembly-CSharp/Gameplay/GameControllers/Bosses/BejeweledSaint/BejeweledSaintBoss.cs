using System;
using System.Collections;
using DG.Tweening;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.BejeweledSaint.Attack;
using Gameplay.GameControllers.Bosses.BejeweledSaint.Audio;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BejeweledSaint
{
	public class BejeweledSaintBoss : MonoBehaviour
	{
		public BejeweledSmashHandManager HandsManager { get; private set; }

		public BsHolderManager HoldersManager { get; private set; }

		public BsDivineBeamManager BeamManager { get; private set; }

		public BejeweledSaintHead Head { get; private set; }

		public BejeweledSaintArmAttack AttackArm { get; private set; }

		public BejeweledSaintCastArm CastArm { get; private set; }

		public BejeweledSaintAudio Audio { get; private set; }

		public bool IsRaised { get; private set; }

		public Vector3 LeftSweepAttackLimitPosition
		{
			get
			{
				return this.LeftSweepAttackLimit.transform.position;
			}
		}

		public Vector3 RightSweepAttackLimitPosition
		{
			get
			{
				return this.RightSweepAttackLimit.transform.position;
			}
		}

		private void Awake()
		{
			this.Head = base.GetComponentInChildren<BejeweledSaintHead>();
			this.AttackArm = base.GetComponentInChildren<BejeweledSaintArmAttack>();
			this.CastArm = base.GetComponentInChildren<BejeweledSaintCastArm>();
			this.HandsManager = UnityEngine.Object.FindObjectOfType<BejeweledSmashHandManager>();
			this.BeamManager = UnityEngine.Object.FindObjectOfType<BsDivineBeamManager>();
			this.HoldersManager = base.GetComponentInChildren<BsHolderManager>();
			this.Audio = base.GetComponentInChildren<BejeweledSaintAudio>();
		}

		private void Start()
		{
			BsHolderManager holdersManager = this.HoldersManager;
			holdersManager.OnBossCollapse = (Core.SimpleEvent)Delegate.Combine(holdersManager.OnBossCollapse, new Core.SimpleEvent(this.OnBossCollapse));
			this.Head.EnableDamageArea(false);
			this.Head.OnDeath += this.OnBossDeath;
			this.IsRaised = true;
		}

		private void OnBossDeath()
		{
			DOTween.Kill(base.transform, false);
			if (this.currentDownfallCoroutine != null)
			{
				base.StopCoroutine(this.currentDownfallCoroutine);
				this.currentDownfallCoroutine = null;
			}
			this.SetSmoothYPos(-20f, 5f, null);
		}

		private void OnBossCollapse()
		{
			this.IsRaised = false;
			DOTween.Kill(base.transform, false);
			this.Head.EnableDamageArea(true);
			this.Audio.PlaySaintFall();
			this.SetSmoothYPos(this.BossHeightPosition.x, 1f, new TweenCallback(this.DownfallLapse));
		}

		private void OnBossRaised()
		{
			if (this.OnRaised != null)
			{
				this.OnRaised();
			}
			this.IsRaised = true;
			this.Head.EnableDamageArea(false);
			this.HoldersManager.EnableHoldersDamageArea(true);
		}

		private void SetSmoothYPos(float yPos, float time, TweenCallback endCallback)
		{
			base.transform.DOLocalMoveY(yPos, time, false).SetEase(Ease.Linear).OnComplete(endCallback).SetId("VerticalMotion");
		}

		public void IntroRaise()
		{
			this.HoldersManager.SetDefaultLocalPositions();
			this.HoldersManager.HealHolders();
			this.HoldersManager.SortRealHolder();
			this.Audio.PlaySaintRise();
			this.SetSmoothYPos(this.BossHeightPosition.y, 2.5f, new TweenCallback(this.OnBossRaisedIntro));
		}

		public void SetIntroPosition()
		{
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, this.BossHeightPosition.x, base.transform.localPosition.z);
		}

		private void OnBossRaisedIntro()
		{
			this.IsRaised = true;
			this.Head.EnableDamageArea(false);
			this.HoldersManager.EnableHoldersDamageArea(true);
		}

		public void DownfallLapse()
		{
			this.currentDownfallCoroutine = base.StartCoroutine(this.DownfallLapseCoroutine());
		}

		private IEnumerator DownfallLapseCoroutine()
		{
			yield return new WaitForSeconds(this.MaxTimeCollapsed);
			if (this.Head.Status.Dead)
			{
				yield break;
			}
			this.HoldersManager.SetDefaultLocalPositions();
			this.HoldersManager.HealHolders();
			this.HoldersManager.SortRealHolder();
			this.Audio.PlaySaintRise();
			this.SetSmoothYPos(this.BossHeightPosition.y, 1f, new TweenCallback(this.OnBossRaised));
			yield break;
		}

		private void OnDestroy()
		{
			this.Head.OnDeath -= this.OnBossDeath;
		}

		public Core.SimpleEvent OnRaised;

		[SerializeField]
		protected GameObject LeftSweepAttackLimit;

		[SerializeField]
		protected GameObject RightSweepAttackLimit;

		[MinMaxSlider(0f, 5f, false)]
		public Vector2 BossHeightPosition;

		public float MaxTimeCollapsed;

		private Coroutine currentDownfallCoroutine;
	}
}
