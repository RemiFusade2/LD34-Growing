using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.IO;
using System;
using UnityEngine.UI;

public class MainBallBehaviour : MonoBehaviour {

	public float mouseClickForce;

	public Texture2D ballTexture;

	public float red;
	public float green;
	public float blue;

	public Renderer sphereRenderer;

	public LayerMask layersForRayCast;

	public GameObject meshColliderGameObject;

	public GameObject currentOrnament;

	// Use this for initialization
	void Start () 
	{
		ballTexture = CreateTexture (30, 30, 1.0f, 1.0f, 1.0f, 1.0f);
		UpdateTexture ();

		gameJoltItemKey = RandomString (30);
	}

	private static System.Random random = new System.Random((int)DateTime.Now.Ticks);
	private string RandomString(int size)
	{
		StringBuilder builder = new StringBuilder ();
		char ch;
		for (int i = 0; i < size; i++) {
			ch = Convert.ToChar (Convert.ToInt32 (Math.Floor (26 * random.NextDouble () + 65)));
			builder.Append (ch);
		}
		return builder.ToString ();
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	void OnMouseDown()
	{
		this.GetComponent<Rigidbody> ().AddForce (mouseClickForce*Vector3.forward);
		this.GetComponent<Rigidbody> ().angularVelocity = Vector3.up * 2;
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

			float deltaTeta = Mathf.PI / 15;
			float deltaRadius = 0.08f;

			Vector3 upVec = this.transform.up;
			Vector3 rightVec = Vector3.Cross(upVec, col.contacts[0].normal);

			float timer = 0;
			float deltaTimer = 0.001f;

			for (float teta = 0 ; teta < Mathf.PI * 2 ; teta += deltaTeta)
			{
				float maxRadius = UnityEngine.Random.Range(0.1f, 0.5f);
				for (float r = 0 ; r < maxRadius ; r += deltaRadius)
				{
					float dx = r * Mathf.Cos(teta);
					float dy = r * Mathf.Sin(teta);
					Vector3 origin = col.contacts[0].point - col.contacts[0].normal + dx * rightVec + dy * upVec; 
					Ray rayToCast = new Ray(origin, col.contacts[0].normal );
					RaycastHit hit;
					if (Physics.Raycast(rayToCast, out hit, 2.0f, layersForRayCast))
					{
						Vector2 textureCoords = hit.textureCoord;
						Color color = new Color(col.collider.GetComponent<ProjectileBehaviour> ().R, col.collider.GetComponent<ProjectileBehaviour> ().G, col.collider.GetComponent<ProjectileBehaviour> ().B);
						StartCoroutine(WaitAndAddColor(timer, color, textureCoords));
						timer += deltaTimer;
					}
				}
			}

			col.collider.GetComponent<ProjectileBehaviour> ().AskForDestroy();
			
			meshColliderGameObject.transform.position = Vector3.zero - 10 * Vector3.up;
			this.GetComponent<Collider>().enabled = true;
		}
	}

	IEnumerator WaitAndAddColor(float timer, Color color, Vector2 textureCoords)
	{
		yield return new WaitForSeconds (timer);
		AddColor (color, textureCoords);
		UpdateTexture();
	}

	public void AddColor(Color color, Vector2 textureCoords)
	{
		try
		{
			int x = Mathf.RoundToInt(textureCoords.x*ballTexture.width);
			int y = Mathf.RoundToInt(textureCoords.y*ballTexture.height);

			Color lastColor = ballTexture.GetPixel(x,y);

			Color newColor = new Color (color.r < lastColor.r ? color.r : lastColor.r, color.g < lastColor.g ? color.g : lastColor.g, color.b < lastColor.b ? color.b : lastColor.b);

			ballTexture.SetPixel(x,y, newColor);
			ballTexture.Apply ();
		}
		catch(UnityException ex)
		{
			Debug.Log(ex.ToString());
		}
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

	private string gameJoltItemKey;
	private const string gameJoltGameID = "113437";

	public GameObject decorationPanelUI;

	public InputField usernameTextInput;
	public InputField messageTextInput;

	public void SaveOrnamentOnGameJolt()
	{
		decorationPanelUI.SetActive (false);

		currentOrnament.GetComponent<OrnamentSphereBehaviour> ().SetAuthorAndMessage (usernameTextInput.text, messageTextInput.text);

		SaveOrnamentOnGameJolt (usernameTextInput.text, messageTextInput.text);
	}

	private int savingRequestCount;

	public void SaveOrnamentOnGameJolt(string username, string message)
	{
		savingRequestCount = 5;

		string gameJoltURL = "http://gamejolt.com/api/game/v1/data-store/set/";

		string gameJoltUsernameURL = gameJoltURL;
		string usernameKey = gameJoltItemKey + "-USERNAME";
		gameJoltUsernameURL += "?game_id=" + gameJoltGameID;
		gameJoltUsernameURL += "&key=" + usernameKey;
		gameJoltUsernameURL += "&data=" + WWW.EscapeURL(username);

		string signature = GenerateSignature (gameJoltUsernameURL);
		gameJoltUsernameURL += "&signature=" + signature;

		Debug.Log (gameJoltUsernameURL);

		WWW www1 = new WWW(gameJoltUsernameURL);
		StartCoroutine(WaitForRequest(www1));


		string gameJoltMessageURL = gameJoltURL;
		string messageKey = gameJoltItemKey + "-MESSAGE";
		gameJoltMessageURL += "?game_id=" + gameJoltGameID;
		gameJoltMessageURL += "&key=" + messageKey;
		gameJoltMessageURL += "&data=" + WWW.EscapeURL(message);

		signature = GenerateSignature (gameJoltMessageURL);
		gameJoltMessageURL += "&signature=" + signature;
		
		Debug.Log (gameJoltMessageURL);

		WWW www2 = new WWW(gameJoltMessageURL);
		StartCoroutine(WaitForRequest(www2));

		
		string gameJoltTextureURL = gameJoltURL;
		string textureKey = gameJoltItemKey + "-TEXTURE";
		gameJoltTextureURL += "?game_id=" + gameJoltGameID;
		gameJoltTextureURL += "&key=" + textureKey;
		gameJoltTextureURL += "&data=";

		for (int x = 0 ; x < ballTexture.width ; x++)
		{
			for (int y = 0 ; y < ballTexture.height ; y++)
			{
				Color currentColor = ballTexture.GetPixel(x,y);
				gameJoltTextureURL += currentColor.r + "," + currentColor.g + "," + currentColor.b + ";";
			}
		}
		
		signature = GenerateSignature (gameJoltTextureURL);
		gameJoltTextureURL += "&signature=" + signature;
		
		Debug.Log (gameJoltTextureURL);
		
		WWW www3 = new WWW(gameJoltTextureURL);
		StartCoroutine(WaitForRequest(www3));


		string gameJoltPositionURL = gameJoltURL;
		string positionKey = gameJoltItemKey + "-POSITION";
		gameJoltPositionURL += "?game_id=" + gameJoltGameID;
		gameJoltPositionURL += "&key=" + positionKey;
		gameJoltPositionURL += "&data=" + WWW.EscapeURL(currentOrnament.transform.position.x+";"+currentOrnament.transform.position.y+";"+currentOrnament.transform.position.z);
		
		signature = GenerateSignature (gameJoltPositionURL);
		gameJoltPositionURL += "&signature=" + signature;
		
		Debug.Log (gameJoltPositionURL);
		
		WWW www4 = new WWW(gameJoltPositionURL);
		StartCoroutine(WaitForRequest(www4));

		
		string gameJoltScaleURL = gameJoltURL;
		string scaleKey = gameJoltItemKey + "-SCALE";
		gameJoltScaleURL += "?game_id=" + gameJoltGameID;
		gameJoltScaleURL += "&key=" + scaleKey;
		gameJoltScaleURL += "&data=" + WWW.EscapeURL(this.transform.localScale.x.ToString());
		
		signature = GenerateSignature (gameJoltScaleURL);
		gameJoltScaleURL += "&signature=" + signature;
		
		Debug.Log (gameJoltScaleURL);
		
		WWW www5 = new WWW(gameJoltScaleURL);
		StartCoroutine(WaitForRequest(www5));
	}

	public GameObject thankYouPanel;

	IEnumerator WaitForRequest(WWW www)
	{
		yield return www;
		// check for errors
		if (www.error == null)
		{
			Debug.Log("WWW Ok!: " + www.text);
			savingRequestCount--;
			if (savingRequestCount <= 0)
			{
				thankYouPanel.SetActive(true);
				StartCoroutine(WaitAndCloseApplication(2.0f));
			}
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}
	}

	IEnumerator WaitAndCloseApplication(float timer)
	{
		yield return new WaitForSeconds (timer);
		Application.Quit ();
	}
	
	private const string gameJoltPrivateKey = "b6eaefd31f816bfa77e1da14791d7dcb";
	private string GenerateSignature(string input)
	{
		MD5 md5 = System.Security.Cryptography.MD5.Create ();
		byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes (input + gameJoltPrivateKey);
		byte[] hash = md5.ComputeHash (inputBytes);
		StringBuilder sb = new StringBuilder ();
		for (int i = 0; i < hash.Length ; i++)
		{
			sb.Append(hash[i].ToString("X2"));
		}
		string signature = sb.ToString().ToLower();
		return signature;
	}
}
