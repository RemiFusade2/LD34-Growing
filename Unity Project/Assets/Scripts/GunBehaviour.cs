using UnityEngine;
using System.Collections;

public class GunBehaviour : MonoBehaviour {

	public GameObject projectilePrefab;

	public Vector3 initialForce;

	public float red;
	public float green;
	public float blue;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Shoot(red,green,blue);
		}
		else if (Input.GetKeyDown(KeyCode.Tab))
		{
			if (red < 0.1f)
			{
				red = 1;
				green = 0;
			}
			else if (green < 0.1f)
			{
				green = 1;
				blue = 0;
			}
			else
			{
				blue = 1;
				red = 0;
			}
		}
	}

	public void Shoot(float R, float G, float B)
	{
		GameObject newProjectile = (GameObject)Instantiate (projectilePrefab, this.transform.position, Quaternion.identity);
		newProjectile.GetComponent<ProjectileBehaviour> ().UpdateTexture (R, G, B);
		newProjectile.GetComponent<Rigidbody> ().velocity = initialForce;
	}
}
