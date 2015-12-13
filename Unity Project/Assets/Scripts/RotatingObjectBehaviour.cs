using UnityEngine;
using System.Collections;

public class RotatingObjectBehaviour : MonoBehaviour {

	public float rotationSpeed;

	// Use this for initialization
	void Start () 
	{	
		this.GetComponent<Rigidbody> ().angularVelocity = new Vector3 (0, rotationSpeed, 0);	
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
}
