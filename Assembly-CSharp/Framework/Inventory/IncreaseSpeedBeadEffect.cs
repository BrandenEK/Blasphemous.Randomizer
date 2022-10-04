using System;
using CreativeSpore.SmartColliders;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;

namespace Framework.Inventory
{
	public class IncreaseSpeedBeadEffect : ObjectEffect
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			this.SaveDefaultMoveSettings(penitent.Dash.DefaultMoveSetting);
		}

		protected override bool OnApplyEffect()
		{
			this.LoadBeadMoveSettings();
			return true;
		}

		protected override void OnRemoveEffect()
		{
			base.OnRemoveEffect();
			this.LoadDefaultMoveSettings();
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
		}

		private void SaveDefaultMoveSettings(Dash.MoveSetting moveSetting)
		{
			this.DefaultMoveSettings = new Dash.MoveSetting(moveSetting.Drag, moveSetting.Speed);
		}

		private void LoadBeadMoveSettings()
		{
			Dash.MoveSetting moveSetting = new Dash.MoveSetting(this.BeadMoveSettings.Drag, this.BeadMoveSettings.Speed);
			Core.Logic.Penitent.Dash.DefaultMoveSetting = moveSetting;
			this.SetPlayerSpeed(moveSetting);
		}

		private void LoadDefaultMoveSettings()
		{
			Core.Logic.Penitent.Dash.DefaultMoveSetting = this.DefaultMoveSettings;
			this.SetPlayerSpeed(this.DefaultMoveSettings);
		}

		private void SetPlayerSpeed(Dash.MoveSetting moveSetting)
		{
			PlatformCharacterController platformCharacterController = Core.Logic.Penitent.PlatformCharacterController;
			if (!platformCharacterController)
			{
				return;
			}
			platformCharacterController.MaxWalkingSpeed = moveSetting.Speed;
			platformCharacterController.WalkingDrag = moveSetting.Drag;
		}

		public Dash.MoveSetting BeadMoveSettings;

		private Dash.MoveSetting DefaultMoveSettings;
	}
}
