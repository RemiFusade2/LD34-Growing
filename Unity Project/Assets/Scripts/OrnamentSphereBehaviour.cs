using UnityEngine;
using System.Collections;

public class OrnamentSphereBehaviour : MonoBehaviour {

	private Texture2D myTexture;

	public string authorName;
	public string authorMessage;

	public void SetAuthorAndMessage(string author, string message)
	{
		authorName = author;
		authorMessage = message;
	}

	public TextMesh text3D;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetTexture(Texture2D newTexture)
	{
		myTexture = newTexture;
		this.GetComponent<Renderer> ().material.mainTexture = myTexture;
	}

	public void SetScale(float scale)
	{
		this.transform.localScale = Vector3.one * scale;
	}

	public void InitializeFromStrings(string name, string message, string texture, string position, string scale)
	{
		authorName = name;
		authorMessage = message;

		if (authorMessage.Length > 25)
		{
			string[] words = authorMessage.Split (' ');
			authorMessage = "";
			int count = 0;
			foreach (string word in words)
			{
				authorMessage += word;
				count += word.Length+1;
				if (count >= 25)
				{
					authorMessage += "\n";
					count = 0;
				}
				else
				{
					authorMessage += " ";
				}
			}
		}

		myTexture = new Texture2D (30, 30);
		string[] textureCoordinatesStrArray = texture.Split (';');
		int x = 0;
		int y = 0;
		foreach (string str in textureCoordinatesStrArray)
		{
			if (str != null && str.Length > 3)
			{
				string[] colorsStr = str.Split(',');
				Color color = new Color(int.Parse(colorsStr[0]), int.Parse(colorsStr[1]), int.Parse(colorsStr[2]));
				myTexture.SetPixel(x,y,color);
				y++;
				if (y >= myTexture.height)
				{
					y = 0;
					x++;
				}
				if (x >= myTexture.width)
				{
					break;
				}
			}
		}
		myTexture.Apply ();
		this.GetComponent<Renderer> ().material.mainTexture = myTexture;

		string[] positionCoords = position.Split (';');
		this.transform.position = new Vector3 (float.Parse(positionCoords[0]), float.Parse(positionCoords[1]), float.Parse(positionCoords[2]) );

		float scaleValue = float.Parse(scale);
		this.transform.localScale = Vector3.one * scaleValue;
	}
	
	void OnMouseEnter()
	{
		if (text3D != null)
		{
			if (authorMessage.Equals("Created by") && authorName.Equals("Maitre Pantoufle"))
			{
				text3D.text = authorMessage + " " + authorName;
			}
			else
			{
				text3D.text = "\"" + authorMessage + "\"\n- " + authorName;
			}
		}
	}
	void OnMouseExit()
	{
		if (text3D != null)
		{
			text3D.text = "";
		}
	}
}
