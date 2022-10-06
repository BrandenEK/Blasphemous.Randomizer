using System;
using System.Linq;
using Gameplay.GameControllers.Entities;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Tools.Playmaker2.Action
{
	[ActionCategory(46)]
	[Tooltip("Find all active GameObjects with a specific name and store them in an array.")]
	public class ArrayFindGameObjectsByName : FsmStateAction
	{
		public override void Reset()
		{
			this.array = null;
			this.ObjectName = new FsmString
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			this.FindGoByName();
			base.Finish();
		}

		public void FindGoByName()
		{
			if (string.IsNullOrEmpty(this.ObjectName.Value) || this.ObjectName.Value.Length <= 1)
			{
				return;
			}
			this.array.Values = (from go in Object.FindObjectsOfType<GameObject>()
			where go.name.Contains(this.ObjectName.Value) && !go.GetComponentInChildren<Enemy>().Status.Dead
			select go).ToArray<GameObject>();
		}

		[RequiredField]
		[UIHint(10)]
		[Tooltip("The Array Variable to use.")]
		public FsmArray array;

		[Tooltip("the name")]
		public FsmString ObjectName;
	}
}
