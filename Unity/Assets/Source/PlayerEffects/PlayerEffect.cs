using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerEffect {

	public enum EffectType
	{
		SHIELD,
		ROCKET_JUMP,
		NUM_TYPES
	}

	protected bool isExpired = false;
	protected float duration;
	protected float durationElapsed;
	protected EffectType effectType;
	protected Player owner;

	public bool IsExpired
	{
		get { return isExpired; }
		set { isExpired = value; }
	}

	public EffectType Type
	{
		get { return effectType; }
		protected set { effectType = value; }
	}

	protected PlayerEffect(float duration = 0f, EffectType type = EffectType.NUM_TYPES, Player owner = null)
	{
		this.duration = duration;
		this.owner = owner;
		durationElapsed = 0f;
		effectType = type;
	}

	public virtual void Update()
	{
		if (duration > 0f && !isExpired)
		{
			durationElapsed += Time.deltaTime;
			if (durationElapsed >= duration)
			{
				OnEffectExpired ();
			}
		}
	}

	protected virtual void OnEffectExpired()
	{
		durationElapsed = 0f;
		isExpired = true;
	}
}
