using System;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.NPCs.BloodDecals
{
	public class PermaBlood : MonoBehaviour
	{
		private void Start()
		{
		}

		public GameObject Instance(Vector3 pos, Quaternion rotation)
		{
			return Object.Instantiate<GameObject>(base.transform.gameObject, pos, rotation);
		}

		public PermaBlood.PermaBloodType permaBloodType;

		public enum PermaBloodType
		{
			permablood_1,
			permablood_2
		}
	}
}
