using System;
using Gameplay.GameControllers.Entities;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Tools.PlayMaker.Action
{
	[ActionCategory(ActionCategory.Array)]
	[HutongGames.PlayMaker.Tooltip("Enables de behaviour of a group of enemies.")]
	public class EnableEnemiesBehaviour : FsmStateAction
	{
		public override void Reset()
		{
			this.array = null;
		}

		public override void OnEnter()
		{
			this.EnableEnemies();
			base.Finish();
		}

		private void EnableEnemies()
		{
			if (this.array.Length < 1)
			{
				return;
			}
			Enemy[] array = UnityEngine.Object.FindObjectsOfType<Enemy>();
			foreach (object obj in this.array.Values)
			{
				foreach (Enemy enemy in array)
				{
					if (enemy.SpawningId == obj.GetHashCode())
					{
						enemy.EnemyBehaviour.EnableBehaviourOnLoad = this.EnableBehaviour.Value;
						if (this.EnableBehaviour.Value)
						{
							enemy.EnemyBehaviour.StartBehaviour();
						}
						else
						{
							enemy.EnemyBehaviour.StopBehaviour();
						}
						break;
					}
				}
			}
		}

		[HutongGames.PlayMaker.Tooltip("The set of enemies.")]
		[ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
		public FsmArray EnemySpawners;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[HutongGames.PlayMaker.Tooltip("The Array Variable to use.")]
		public FsmArray array;

		public FsmBool EnableBehaviour;
	}
}
