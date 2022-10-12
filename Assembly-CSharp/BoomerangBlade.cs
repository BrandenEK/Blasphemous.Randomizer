using System;
using FMOD.Studio;
using FMODUnity;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Sirenix.OdinInspector;
using UnityEngine;

public class BoomerangBlade : Weapon
{
	public AttackArea AttackArea { get; private set; }

	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	protected override void OnAwake()
	{
		base.OnAwake();
		this.AttackArea = base.GetComponentInChildren<AttackArea>();
	}

	protected override void OnStart()
	{
		base.OnStart();
		this.AttackArea.OnEnter += this.AttackAreaOnEnter;
	}

	public new void SetHit(Hit hit)
	{
		this._hit = hit;
		this._hit.HitSoundId = this.hitSound;
	}

	private void AttackAreaOnEnter(object sender, Collider2DParam collider2DParam)
	{
		this.Attack(this._hit);
	}

	public override void Attack(Hit weapondHit)
	{
		base.GetDamageableEntities();
		base.AttackDamageableEntities(weapondHit);
	}

	public override void OnHit(Hit weaponHit)
	{
	}

	public void SetOwner(Enemy enemy)
	{
		this.AttackingEntity = enemy;
		this.WeaponOwner = enemy;
	}

	private void OnDestroy()
	{
		this.StopFlightFx();
		this.AttackArea.OnEnter -= this.AttackAreaOnEnter;
	}

	public void PlayFlightFx()
	{
		if (this._flyingBladeAudioInstance.isValid())
		{
			this.StopFlightFx();
		}
		this._flyingBladeAudioInstance = Core.Audio.CreateEvent(this.flightSound, default(Vector3));
		this._flyingBladeAudioInstance.start();
	}

	public void StopFlightFx()
	{
		if (!this._flyingBladeAudioInstance.isValid())
		{
			return;
		}
		this._flyingBladeAudioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		this._flyingBladeAudioInstance.release();
		this._flyingBladeAudioInstance = default(EventInstance);
	}

	public override void OnObjectReuse()
	{
		base.OnObjectReuse();
		this.PlayFlightFx();
	}

	public void Recycle()
	{
		this.StopFlightFx();
		base.Destroy();
	}

	private Animator _animator;

	public Enemy AttackingEntity;

	[SerializeField]
	[BoxGroup("Audio", true, false, 0)]
	[EventRef]
	private string hitSound;

	[SerializeField]
	[BoxGroup("Audio", true, false, 0)]
	[EventRef]
	private string flightSound;

	private Hit _hit;

	private EventInstance _flyingBladeAudioInstance;
}
