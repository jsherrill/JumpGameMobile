using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEffect : PlayerEffect {
	public ShieldEffect(float duration = 0f, Player owner = null) : base(duration, EffectType.SHIELD, owner)
	{
	}
}
