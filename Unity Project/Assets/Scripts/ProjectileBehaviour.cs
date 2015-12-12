using UnityEngine;
using System.Collections;

public class ProjectileBehaviour : MonoBehaviour {


	public float R;
	public float G;
	public float B;

	public Texture2D mainTexture;

	// Use this for initialization
	void Start () 
	{
		UpdateTexture (R, G, B);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void UpdateTexture(float red, float green, float blue)
	{
		R = red;
		G = green;
		B = blue;
		mainTexture = new Texture2D (100, 100);
		for (int x = 0 ; x < mainTexture.width ; x++)
		{
			for (int y = 0 ; y < mainTexture.height ; y++)
			{
				mainTexture.SetPixel(x,y, new Color(red, green, blue, 1.0f));
			}
		}
		mainTexture.Apply ();
		this.GetComponent<Renderer> ().material.mainTexture = mainTexture;
	}

	public void AskForDestroy()
	{
		StartCoroutine (WaitAndDestroy (0));
	}

	IEnumerator WaitAndDestroy(float timer)
	{
		yield return new WaitForSeconds (timer);

		Destroy (this.gameObject);
	}
}
