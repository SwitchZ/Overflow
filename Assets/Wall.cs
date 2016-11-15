using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {
    private bool canMove = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        this.GetComponent<Renderer>().material.mainTextureScale = new Vector2(transform.localScale.x, transform.localScale.y);

        if (ScoreManager.instance.round == 4) //walls on stage 4 are oscillable like enemies on it
        { 
            if (canMove) StartCoroutine(SineWalls()); 
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Bullet" || other.gameObject.tag == "EnemyBullet") Destroy(other.gameObject);
    }

    IEnumerator SineWalls() //space invader style; sine movement and shoot
    {
        canMove = false;

        //left 
        transform.position = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
        yield return new WaitForSeconds(1f);
        //...and right
        transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
        yield return new WaitForSeconds(1f);

        canMove = true;
    }
}
