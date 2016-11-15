using UnityEngine;
using System.Collections;

public class Ground : MonoBehaviour {
    public float scrollSpeed = -2f;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        /*
        if (ScoreManager.instance.round == 8) {
            float offset = Time.time * scrollSpeed;
            transform.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offset, 0);
        }
        */
	}
}
