using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using UnityEngine.UI;

public class ChristmasTreeBehaviour : MonoBehaviour {

	private int numberOfKeysToGet;

	public float maxTimeToWaitForRequests;

	private Dictionary<string, string> keyDataDico;

	private Coroutine checkForData;

	private bool dataLoaded;

	public GameObject ornamentSpherePrefab;

	public bool readyToPutOrnament;

	// Use this for initialization
	void Start () 
	{
		keyDataDico = new Dictionary<string, string> ();
		dataLoaded = false;

		FetchKeys ();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public GameObject currentDecoration;
	public bool hasBeenPutOntree;

	void OnMouseDown()
	{
		//FetchKeys ();
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (currentDecoration != null && !hasBeenPutOntree && readyToPutOrnament && Physics.Raycast(ray, out hit))
		{
			if (hit.collider.tag.Equals("Tree"))
			{
				AddDecoration(hit.point, currentDecoration);
				hasBeenPutOntree = true;
			}
		}
	}

	public Button submitDecorationButton;

	public void AddDecoration(Vector3 localPosition, GameObject decoration)
	{
		decoration.transform.parent = this.transform;
		decoration.transform.position = localPosition;
		decoration.transform.localScale *= 5;
		submitDecorationButton.interactable = true;
	}

	public void FetchKeys()
	{
		keyDataDico.Clear ();

		string fetchKeysURL = "http://gamejolt.com/api/game/v1/data-store/get-keys/";

		fetchKeysURL += "?game_id=" + gameJoltGameID;
		
		string signature = GenerateSignature (fetchKeysURL);
		fetchKeysURL += "&signature=" + signature;
		
		Debug.Log (fetchKeysURL);
		
		WWW www1 = new WWW(fetchKeysURL);
		StartCoroutine(WaitForRequest(www1));

		checkForData = StartCoroutine (WaitAndCheckForData (maxTimeToWaitForRequests));
	}

	IEnumerator WaitAndCheckForData(float timer)
	{
		yield return new WaitForSeconds (timer);
		if (!dataLoaded)
		{
			// Can't load data
			Debug.Log("Waiting time was too long");
		}
	}
	
	private string gameJoltItemKey;
	private const string gameJoltGameID = "113437";
	
	IEnumerator WaitForRequest(WWW www)
	{
		yield return www;
		// check for errors
		if (www.error == null)
		{
			//Debug.Log("WWW Ok!: " + www.text);

			if (www.text.Contains("key:\""))
			{
				ComputeKeys(www.text);
			} 
			else if (www.text.Contains("data:\""))
			{
				ComputeData(www.url, www.text);
			}
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}    
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

	private void ComputeKeys(string inputString)
	{
		string[] splitStr = inputString.Split ('\n');

		numberOfKeysToGet = 0;
		foreach (string str in splitStr)
		{
			if (str.Contains("key:\""))
			{
				numberOfKeysToGet++;
				string key = str.Substring(5, str.Length-7);
				FetchDataForKey(key);
			}
		}
	}

	private void FetchDataForKey(string key)
	{
		string fetchKeyURL = "http://gamejolt.com/api/game/v1/data-store/";
		
		fetchKeyURL += "?game_id=" + gameJoltGameID;
		fetchKeyURL += "&key=" + key;
		
		string signature = GenerateSignature (fetchKeyURL);
		fetchKeyURL += "&signature=" + signature;
		
		Debug.Log (fetchKeyURL);
		
		WWW www1 = new WWW(fetchKeyURL);
		StartCoroutine(WaitForRequest(www1));
	}

	private void ComputeData(string url, string data)
	{
		int indexStartKey = url.IndexOf ("&key=") + 5;
		int indexEndKey = url.IndexOf ("&signature=");
		string key = url.Substring( indexStartKey, indexEndKey-indexStartKey );

		int dataStartIndex = data.IndexOf ("data:\"") + 6;
		string dataClean = data.Substring (dataStartIndex, data.Length - dataStartIndex - 3);

		keyDataDico.Add (key, dataClean);

		CreateAllOrnamentSpheres ();
	}

	public TextMesh text3D;

	private void CreateAllOrnamentSpheres ()
	{
		if (keyDataDico.Count >= numberOfKeysToGet)
		{
			dataLoaded = true;
			StopCoroutine(checkForData);
			
			Dictionary<string, Dictionary<string,string>> dataDico = new Dictionary<string, Dictionary<string,string>> ();
			foreach(string key in keyDataDico.Keys)
			{
				string keyName = key.Split('-')[0];
				string keyContent = key.Split('-')[1];

				Dictionary<string,string> contentDico = null;
				if (!dataDico.ContainsKey(keyName))
				{
					contentDico = new Dictionary<string,string>();
					contentDico.Add(keyContent, keyDataDico[key]);
					dataDico.Add(keyName, contentDico);
				}
				else
				{
					contentDico = dataDico[keyName];
					contentDico.Add(keyContent, keyDataDico[key]);
				}
			}

			foreach (string keyName in dataDico.Keys)
			{
				Dictionary<string,string> contentDico = dataDico[keyName];
				GameObject instance = (GameObject) Instantiate (ornamentSpherePrefab, Vector3.zero, Quaternion.identity);
				instance.transform.parent = this.transform;
				instance.GetComponent<OrnamentSphereBehaviour>().InitializeFromStrings(contentDico["USERNAME"], contentDico["MESSAGE"], contentDico["TEXTURE"], contentDico["POSITION"], contentDico["SCALE"]);
				instance.GetComponent<OrnamentSphereBehaviour>().text3D = text3D;			
			}
		}
	}
}
