using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Level.Actionables
{
	public class BasicTrap : MonoBehaviour, IActionable
	{
		private void Awake()
		{
			this.sensor.OnEntityEnter += this.OnTargetEnter;
			this.sensor.OnEntityExit += this.OnTargetExit;
			this.animationEvent.OnEventLaunched += this.OnEventLaunched;
			this.animator.speed = this.animationSpeed;
			if (!string.IsNullOrEmpty(this.activationSound))
			{
				this.activationSoundInstnace = Core.Audio.CreateEvent(this.activationSound, default(Vector3));
			}
			if (!string.IsNullOrEmpty(this.deactivationSound))
			{
				this.deactivationSoundInstance = Core.Audio.CreateEvent(this.deactivationSound, default(Vector3));
			}
			if (!string.IsNullOrEmpty(this.anticipationEndSound))
			{
				this.anticipationEndSoundInstnace = Core.Audio.CreateEvent(this.anticipationEndSound, default(Vector3));
			}
			this.realReactivationTime = this.reactivationTime;
			AnimationClip animationClip = this.FindAnticipationClip();
			if (animationClip != null)
			{
				this.anticipationAnimExists = true;
				this.realReactivationTime = this.CalculateRealReactivationTime(animationClip);
			}
		}

		private void OnDestroy()
		{
			if (this.sensor)
			{
				this.sensor.OnEntityEnter -= this.OnTargetEnter;
				this.sensor.OnEntityExit -= this.OnTargetExit;
			}
			if (this.animationEvent)
			{
				this.animationEvent.OnEventLaunched -= this.OnEventLaunched;
			}
		}

		private IEnumerator Start()
		{
			this.mainCamera = Camera.main;
			if (!this.startActive)
			{
				yield break;
			}
			BasicTrap.TimeSyncType timeSyncType = this.syncType;
			if (timeSyncType != BasicTrap.TimeSyncType.Delay)
			{
				if (timeSyncType == BasicTrap.TimeSyncType.Offset)
				{
					float animationDuration = this.animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
					float normalizedTime = (animationDuration - this.activationDelay) / animationDuration;
					this.animator.Play(0, 0, normalizedTime);
					this.Use();
				}
			}
			else
			{
				yield return new WaitForSeconds(this.activationDelay);
				this.Use();
			}
			yield break;
		}

		private void Update()
		{
			if (this.activationSoundInstnace.isValid())
			{
				Core.Audio.ModifyPanning(this.activationSoundInstnace, base.transform);
			}
			if (this.deactivationSoundInstance.isValid())
			{
				Core.Audio.ModifyPanning(this.deactivationSoundInstance, base.transform);
			}
			if (this.anticipationEndSoundInstnace.isValid())
			{
				Core.Audio.ModifyPanning(this.anticipationEndSoundInstnace, base.transform);
			}
			this.UpdateAnimatorSpeed();
			if (this.damaging && this.sensor.EntitiesInside)
			{
				if (Core.Logic.Penitent.Status.Unattacable && !this.DamageWhileUnattacable)
				{
					return;
				}
				this.DamageTargets();
				this.damaging = false;
			}
			if (this.waitingForActivationSound && this.spriteRenderer.IsVisibleFrom(this.mainCamera))
			{
				this.waitingForActivationSound = false;
				if (!string.IsNullOrEmpty(this.activationSound))
				{
					this.activationSoundInstnace.start();
				}
			}
		}

		private void UpdateAnimatorSpeed()
		{
			if (!this.anticipationAnimExists || this.anticipationClipSpeed == 1f)
			{
				return;
			}
			if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("ANTICIPATION"))
			{
				this.animator.speed = this.anticipationClipSpeed;
			}
			else
			{
				this.animator.speed = this.animationSpeed;
			}
		}

		private void OnEventLaunched(string id)
		{
			if (id != null)
			{
				if (!(id == "DAMAGE_START"))
				{
					if (!(id == "DAMAGE_END"))
					{
						if (!(id == "TRAP_RETREAT"))
						{
							if (!(id == "LOOP_END"))
							{
								if (id == "ANTICIPATION_END")
								{
									this.OnAnticipationEnd();
								}
							}
							else
							{
								this.OnUseFinish();
							}
						}
						else
						{
							this.OnTrapRetreat();
						}
					}
					else
					{
						this.damaging = false;
					}
				}
				else
				{
					this.damaging = true;
				}
			}
		}

		private void OnTargetEnter(Entity entity)
		{
			if (!this.targets.Contains(entity))
			{
				this.targets.Add(entity);
			}
		}

		private void OnTargetExit(Entity entity)
		{
			if (this.targets.Contains(entity))
			{
				this.targets.Remove(entity);
			}
		}

		public void Use()
		{
			if (!this.Locked)
			{
				base.StartCoroutine(this.OnUse());
			}
		}

		private IEnumerator OnUse()
		{
			if (this.state != BasicTrap.TrapState.Idle)
			{
				yield break;
			}
			if (this.firstActivation && !this.startActive)
			{
				yield return new WaitForSeconds(this.activationDelay);
				this.firstActivation = false;
			}
			this.state = BasicTrap.TrapState.Moving;
			this.NotifyAnimator(true);
			this.Locked = true;
			this.waitingForActivationSound = true;
			yield break;
		}

		private void OnUseFinish()
		{
			this.state = BasicTrap.TrapState.Idle;
			this.NotifyAnimator(false);
			this.Locked = false;
			this.waitingForActivationSound = false;
			if (this.looping)
			{
				base.Invoke("Use", this.realReactivationTime);
			}
		}

		private void NotifyAnimator(bool active)
		{
			if (this.anticipationAnimExists)
			{
				if (active)
				{
					this.animator.SetTrigger("ACTIVATE");
				}
				else
				{
					this.animator.SetTrigger("DEACTIVATE");
				}
			}
			else
			{
				this.animator.SetBool("ACTIVE", active);
			}
		}

		private AnimationClip FindAnticipationClip()
		{
			List<AnimationClip> list = new List<AnimationClip>(this.animator.runtimeAnimatorController.animationClips);
			return list.Find((AnimationClip x) => x.name.EndsWith("ANTICIPATION", StringComparison.InvariantCultureIgnoreCase));
		}

		private float CalculateRealReactivationTime(AnimationClip anticipationClip)
		{
			float result = 0f;
			float length = anticipationClip.length;
			if (this.reactivationTime - length > 0f)
			{
				result = this.reactivationTime - length;
			}
			else
			{
				this.anticipationClipSpeed = length / this.reactivationTime;
			}
			return result;
		}

		private void OnTrapRetreat()
		{
			if (this.spriteRenderer.IsVisibleFrom(this.mainCamera) && !string.IsNullOrEmpty(this.deactivationSound))
			{
				this.deactivationSoundInstance.start();
			}
		}

		private void OnAnticipationEnd()
		{
			if (this.spriteRenderer.IsVisibleFrom(this.mainCamera) && !string.IsNullOrEmpty(this.anticipationEndSound))
			{
				this.anticipationEndSoundInstnace.start();
			}
		}

		private void DamageTargets()
		{
			if (this.targets.Count == 0)
			{
				return;
			}
			Hit hit = new Hit
			{
				AttackingEntity = base.gameObject,
				Unnavoidable = true,
				DamageAmount = (float)this.damage,
				DamageType = this.damageType,
				Force = 0f,
				HitSoundId = this.HitSound
			};
			for (int i = 0; i < this.targets.Count; i++)
			{
				IDamageable componentInParent = this.targets[i].GetComponentInParent<IDamageable>();
				componentInParent.Damage(hit);
			}
		}

		public bool Locked { get; set; }

		private const string DAMAGE_START_EVENT = "DAMAGE_START";

		private const string DAMAGE_END_EVENT = "DAMAGE_END";

		private const string LOOP_END_EVENT = "LOOP_END";

		private const string TRAP_RETREAT_EVENT = "TRAP_RETREAT";

		private const string ANTICIPATION_STRING = "ANTICIPATION";

		private const string ANTICIPATION_END_EVENT = "ANTICIPATION_END";

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private bool looping = true;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private bool DamageWhileUnattacable = true;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private BasicTrap.TimeSyncType syncType;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		[Range(0f, 60f)]
		private float activationDelay = 1f;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		[Range(0f, 4f)]
		private float reactivationTime = 1f;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private int damage = 1;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		public bool startActive = true;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private DamageArea.DamageType damageType;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		[Range(0.5f, 2f)]
		private float animationSpeed = 1f;

		[SerializeField]
		[BoxGroup("Events", true, false, 0)]
		private string OnDamage;

		[SerializeField]
		[BoxGroup("Events", true, false, 0)]
		private string OnHurtModeEnter;

		[SerializeField]
		[BoxGroup("Events", true, false, 0)]
		private string OnHurtModeExit;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[EventRef]
		private string activationSound;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[EventRef]
		private string deactivationSound;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[EventRef]
		private string anticipationEndSound;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[EventRef]
		private string HitSound;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		private Animator animator;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		private SpriteRenderer spriteRenderer;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		private AnimatorEvent animationEvent;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		private TriggerSensor sensor;

		private List<Entity> targets = new List<Entity>();

		private BasicTrap.TrapState state;

		private bool damaging;

		private EventInstance activationSoundInstnace;

		private EventInstance deactivationSoundInstance;

		private EventInstance anticipationEndSoundInstnace;

		private bool firstActivation = true;

		private bool waitingForActivationSound;

		private float anticipationClipSpeed = 1f;

		private float realReactivationTime;

		private bool anticipationAnimExists;

		private Camera mainCamera;

		private enum TrapState
		{
			Idle,
			Moving
		}

		private enum TimeSyncType
		{
			Delay,
			Offset
		}
	}
}
