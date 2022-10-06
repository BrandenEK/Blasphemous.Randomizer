using System;
using Extras.EOS.Data;
using UnityEngine;

namespace Extras.EOS
{
	public class EOSSDKComponent : MonoBehaviour
	{
		private void Awake()
		{
			Object.Destroy(base.gameObject);
		}

		[SerializeField]
		private EOSConnectionInfoData _eosConnectionInfo;
	}
}
