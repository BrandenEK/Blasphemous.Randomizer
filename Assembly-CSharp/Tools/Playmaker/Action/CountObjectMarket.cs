using System;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Tools.PlayMaker.Action
{
	[ActionCategory("Blasphemous Action")]
	public class CountObjectMarket : FsmStateAction
	{
		public override void Reset()
		{
			this.TotalObj = null;
		}

		public override void OnEnter()
		{
		}

		public override void OnUpdate()
		{
			this.TotalObj = GameObject.FindGameObjectsWithTag("MKTObj").Length;
		}

		public override void OnExit()
		{
		}

		public FsmInt TotalObj;

		private GameObject objetos;
	}
}
