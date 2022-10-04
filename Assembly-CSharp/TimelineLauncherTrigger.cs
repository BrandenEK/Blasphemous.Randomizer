using System;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineLauncherTrigger : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (this.alreadyPlayed && !this.repeat)
		{
			return;
		}
		if (this.collisionMask == (this.collisionMask | 1 << collision.gameObject.layer) && this.timelinePlayableDirector)
		{
			this.timelinePlayableDirector.Play();
			this.alreadyPlayed = true;
		}
	}

	public LayerMask collisionMask;

	public PlayableDirector timelinePlayableDirector;

	public bool repeat;

	private bool alreadyPlayed;
}
