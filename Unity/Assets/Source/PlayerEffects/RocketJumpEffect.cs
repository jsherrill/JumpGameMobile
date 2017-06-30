using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketJumpEffect : PlayerEffect {
	private float maxMomentum = 25f;

	public float MaxMomentum
	{
		get { return maxMomentum; }
		set { maxMomentum = value; }
	}

	public RocketJumpEffect(float duration = 0f, float maxMomentum = 0f, Player owner = null) : base(duration, EffectType.ROCKET_JUMP, owner)
	{
		this.maxMomentum = maxMomentum;
	}

	public override void Update ()
	{
		base.Update ();

		if (owner != null)
		{
			if (!isExpired)
			{
				owner.CurrentMomentum = Mathf.Lerp (owner.CurrentMomentum, maxMomentum, Time.deltaTime);
			}
		}
	}

	protected override void OnEffectExpired ()
	{
		base.OnEffectExpired ();

		if (owner != null)
		{
			owner.CurrentMomentum = owner.InitialJumpForce;
		}
	}
}
