using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticlesOnIdleTrampoline : MonoBehaviour {

    public ParticleSystem idleTrampoline;

	public void ThrowParticles()
    {
        idleTrampoline.Play();
    }
}
