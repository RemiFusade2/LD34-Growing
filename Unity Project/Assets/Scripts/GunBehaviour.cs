using UnityEngine;
using System.Collections;

public class GunBehaviour : MonoBehaviour {

	public GameObject projectilePrefab;

	public Vector3 initialForce;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Shoot(1,0,0);
		}
	}

	public void Shoot(float R, float G, float B)
	{
		GameObject newProjectile = (GameObject)Instantiate (projectilePrefab, this.transform.position, Quaternion.identity);
		newProjectile.GetComponent<ProjectileBehaviour> ().UpdateTexture (0, 1.0f, 0);
		newProjectile.GetComponent<Rigidbody> ().velocity = initialForce;
	}
}
