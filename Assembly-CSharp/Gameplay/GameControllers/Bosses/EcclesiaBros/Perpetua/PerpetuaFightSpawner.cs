using System;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.EcclesiaBros.Perpetua
{
	public class PerpetuaFightSpawner : MonoBehaviour
	{
		private void Start()
		{
			this.LoadPerpetua();
		}

		private void LoadPerpetua()
		{
			this.perpetua = Object.Instantiate<GameObject>(this.perpetuaPrefab, base.transform.position, Quaternion.identity).GetComponent<Perpetua>();
			this.perpetua.transform.SetParent(base.transform.parent);
			this.perpetua.gameObject.SetActive(false);
		}

		public void DismissPerpetua()
		{
			if (this.IsSpawned && this.perpetua)
			{
				this.perpetua.Behaviour.Death();
			}
		}

		public void SpawnFightInPosition(Vector2 pos)
		{
			this.IsSpawned = true;
			this.perpetua.transform.position = new Vector2(pos.x, this.initPerpetuaPosition.position.y);
			this.perpetua.gameObject.SetActive(true);
			this.perpetua.PerpetuaPoints = base.GetComponentInChildren<PerpetuaPoints>();
			this.debugIcon.SetActive(true);
			this.perpetua.GetComponent<PerpetuaBehaviour>().InitIntro();
		}

		public void SpawnFight()
		{
			this.perpetua.transform.position = this.initPerpetuaPosition.position;
			this.perpetua.gameObject.SetActive(true);
			this.perpetua.PerpetuaPoints = base.GetComponentInChildren<PerpetuaPoints>();
			this.debugIcon.SetActive(true);
			this.perpetua.GetComponent<PerpetuaBehaviour>().InitIntro();
		}

		public GameObject perpetuaPrefab;

		public Perpetua perpetua;

		public Transform initPerpetuaPosition;

		public GameObject debugIcon;

		private bool IsSpawned;
	}
}
