using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay.UI.Others.MenuLogic
{
	public class CustomEventInput : MonoBehaviour
	{
		public GameObject CurrentButton
		{
			get
			{
				return this.currentButton;
			}
		}

		private void Update()
		{
			if (this.timer <= 0f)
			{
				this.currentAxis = new AxisEventData(EventSystem.current);
				this.currentButton = EventSystem.current.currentSelectedGameObject;
				if (Input.GetAxisRaw("Vertical") > 0f)
				{
					this.currentAxis.moveDir = MoveDirection.Up;
					ExecuteEvents.Execute<IMoveHandler>(this.currentButton, this.currentAxis, ExecuteEvents.moveHandler);
				}
				else if (Input.GetAxisRaw("Vertical") < 0f)
				{
					this.currentAxis.moveDir = MoveDirection.Down;
					ExecuteEvents.Execute<IMoveHandler>(this.currentButton, this.currentAxis, ExecuteEvents.moveHandler);
				}
				else if (Input.GetAxis("Horizontal") > this.deadZone)
				{
					this.currentAxis.moveDir = MoveDirection.Right;
					ExecuteEvents.Execute<IMoveHandler>(this.currentButton, this.currentAxis, ExecuteEvents.moveHandler);
				}
				else if (Input.GetAxis("Horizontal") < -this.deadZone)
				{
					this.currentAxis.moveDir = MoveDirection.Left;
					ExecuteEvents.Execute<IMoveHandler>(this.currentButton, this.currentAxis, ExecuteEvents.moveHandler);
				}
				this.timer = this.timeBetweenInputs;
			}
			this.timer -= Time.deltaTime;
		}

		private GameObject currentButton;

		private AxisEventData currentAxis;

		public float timeBetweenInputs = 0.15f;

		[Range(0f, 1f)]
		public float deadZone = 0.15f;

		private float timer;
	}
}
