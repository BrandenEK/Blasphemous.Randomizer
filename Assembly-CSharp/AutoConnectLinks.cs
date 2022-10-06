using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class AutoConnectLinks : MonoBehaviour
{
	[Button(0)]
	public void ConnectAllJoints()
	{
		List<CharacterJoint> list = new List<CharacterJoint>(base.GetComponentsInChildren<CharacterJoint>());
		foreach (CharacterJoint characterJoint in list)
		{
			if (characterJoint.transform.parent != null)
			{
				characterJoint.connectedBody = characterJoint.transform.parent.GetComponent<Rigidbody>();
			}
		}
	}
}
