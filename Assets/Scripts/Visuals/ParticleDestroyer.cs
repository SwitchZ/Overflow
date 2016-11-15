using UnityEngine;
using System.Collections;

public class ParticleDestroyer : MonoBehaviour {


	void Update () {
        if(!this.GetComponent<ParticleSystem>().IsAlive()) Destroy(this.gameObject);
    }


}
