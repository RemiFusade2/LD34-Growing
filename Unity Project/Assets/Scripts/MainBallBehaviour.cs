using UnityEngine;
using System.Collections;

public class MainBallBehaviour : MonoBehaviour {

	public float mouseClickForce;

	public Texture2D ballTexture;

	public float red;
	public float green;
	public float blue;

	public Renderer sphereRenderer;

	public LayerMask layersForRayCast;

	public GameObject meshColliderGameObject;

	// Use this for initialization
	void Start () 
	{
		ballTexture = CreateTexture (100, 100, 1.0f, 1.0f, 1.0f, 1.0f);
		UpdateTexture ();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnMouseDown()
	{
		this.GetComponent<Rigidbody> ().AddForce (mouseClickForce*Vector3.forward);

	}

	void OnCollisionEnter(Collision col)
	{
		if (col.collider.tag.Equals("Projectile"))
		{
			this.GetComponent<Collider>().enabled = false;

			meshColliderGameObject.transform.position = this.transform.position;
			meshColliderGameObject.transform.rotation = this.transform.rotation;

			float currentVolume = 4/3.0f * Mathf.PI * this.transform.localScale.x * this.transform.localScale.y * this.transform.localScale.z;
			float projectileVolume = 4/3.0f * Mathf.PI * col.collider.transform.localScale.x * col.collider.transform.localScale.y * col.collider.transform.localScale.z;

			float newVolume = currentVolume + projectileVolume;

			float newRadius = Mathf.Pow( newVolume * (3.0f/4) * (1/Mathf.PI), 1/3.0f);

			this.transform.localScale = newRadius * Vector3.one;
			meshColliderGameObject.transform.localScale = newRadius * Vector3.one;

			Ray rayToCast = new Ray(col.contacts[0].point - col.contacts[0].normal, col.contacts[0].normal );

			Debug.Log("TODO: raycast on a cone")

			RaycastHit hit;
			if (Physics.Raycast(rayToCast, out hit, 200.0f, layersForRayCast))
			{
				Vector2 textureCoords = hit.textureCoord;
				Color color = new Color(col.collider.GetComponent<ProjectileBehaviour> ().R, col.collider.GetComponent<ProjectileBehaviour> ().G, col.collider.GetComponent<ProjectileBehaviour> ().B);
				AddColor(color, textureCoords);
			}

			UpdateTexture();

			col.collider.GetComponent<ProjectileBehaviour> ().AskForDestroy();
			
			meshColliderGameObject.transform.position = Vector3.zero - 10 * Vector3.up;
			this.GetComponent<Collider>().enabled = true;
		}
	}

	public void AddColor(Color color, Vector2 textureCoords)
	{
		int x = Mathf.RoundToInt(textureCoords.x*ballTexture.width);
		int y = Mathf.RoundToInt(textureCoords.y*ballTexture.height);
		
		Debug.Log ("Add Color at "+x + "," + y);
		ballTexture.SetPixel(x,y, color);
		ballTexture.Apply ();
	}

	public void UpdateTexture()
	{
		sphereRenderer.material.mainTexture = ballTexture;
	}

	public Texture2D CreateTexture(int width, int height, float red, float green, float blue, float alpha)
	{
		Texture2D resTexture = new Texture2D (width, height);
		for (int x = 0 ; x < resTexture.width ; x++)
		{
			for (int y = 0 ; y < resTexture.height ; y++)
			{
				resTexture.SetPixel(x,y, new Color(red, green, blue, alpha));
			}
		}
		resTexture.Apply ();
		return resTexture;
	}
}
