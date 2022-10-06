using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BeamLauncher : MonoBehaviour
{
	private void Start()
	{
		this.results = new RaycastHit2D[1];
		this.CreateBeamBodies();
	}

	private void CreateBeamBodies()
	{
		this.beamParts = new List<GameObject>();
		int num = Mathf.RoundToInt(this.maxRange / this.distanceBetweenBodySprites);
		for (int i = 0; i < num; i++)
		{
			GameObject item = Object.Instantiate<GameObject>(this.beamBodyPrefab, base.transform);
			this.beamParts.Add(item);
		}
	}

	private void Update()
	{
		this.LaunchBeam();
	}

	[Button(0)]
	public void TriggerBeamBodyAnim()
	{
		foreach (GameObject gameObject in this.beamParts)
		{
			if (gameObject.activeInHierarchy)
			{
				Animator componentInChildren = gameObject.GetComponentInChildren<Animator>();
				if (componentInChildren != null)
				{
					componentInChildren.SetTrigger("BEAM");
				}
			}
		}
	}

	private void LaunchBeam()
	{
		Vector2 vector = Vector2.zero;
		if (Physics2D.RaycastNonAlloc(base.transform.position, base.transform.right, this.results, this.maxRange, this.beamCollisionMask) > 0)
		{
			vector = this.results[0].point;
			this.endSprite.position = vector;
			this.endSprite.gameObject.SetActive(true);
		}
		else
		{
			this.endSprite.gameObject.SetActive(false);
			vector = base.transform.position + base.transform.right * this.maxRange;
		}
		this.DrawBeam(base.transform.position, vector);
	}

	private void DrawBeam(Vector2 origin, Vector2 end)
	{
		Vector2 vector = end - origin;
		float magnitude = vector.magnitude;
		int num = Mathf.RoundToInt(magnitude / this.distanceBetweenBodySprites);
		for (int i = 0; i < num; i++)
		{
			Vector2 vector2 = Vector2.Lerp(origin, end, (float)i / (float)num);
			Debug.DrawLine(vector2 - vector.normalized * 0.5f, vector2 + vector.normalized * 0.5f, Color.cyan);
			GameObject gameObject = this.beamParts[i];
			gameObject.SetActive(true);
			gameObject.transform.position = vector2;
			gameObject.transform.right = vector;
		}
		if (num < this.beamParts.Count)
		{
			for (int j = num; j < this.beamParts.Count; j++)
			{
				this.beamParts[j].SetActive(false);
			}
		}
	}

	private RaycastHit2D[] results;

	public float maxRange;

	public LayerMask beamCollisionMask;

	public float distanceBetweenBodySprites;

	public Transform endSprite;

	public List<GameObject> beamParts;

	public GameObject beamBodyPrefab;
}
