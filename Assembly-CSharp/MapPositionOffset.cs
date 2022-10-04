using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MapPositionOffset : MonoBehaviour
{
	[SerializeField]
	private Vector3 mapOffset = Vector3.zero;
}
