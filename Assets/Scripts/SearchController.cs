using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SearchController : MonoBehaviour
{

	public string m_apiKey;
	public InputField m_searchField;
	public Image m_image;

	void Start()
	{
		if (m_apiKey == null || m_searchField == null || m_image == null)
		{
			Debug.Log("missing UI!");			
		}
	}
	
	// Use this for initialization
	IEnumerator Search ()
	{
		var query = m_searchField.text;
		
		UnityWebRequest www = UnityWebRequest.Get("https://pixabay.com/api/?key="+m_apiKey+"&q=" + query);

		yield return www.SendWebRequest();

		if (www.isNetworkError || www.isHttpError)
		{
			Debug.Log("The search request failed");
		}
		else
		{
			var response = www.downloadHandler.text;
			var dict = MiniJSON.Json.Deserialize(response) as Dictionary<string, object>;
			var hits = dict["hits"] as List<object>;
			if (hits != null && hits.Count > 0)
			{
				var hit = hits[0] as Dictionary<string, object>;
				if (hit != null)
				{
					var largeImageURL = hit["largeImageURL"] as string;
					if (largeImageURL != null)
					{
						StartCoroutine("LoadImage", largeImageURL);
						Debug.Log(largeImageURL);
					}
				}
			}
		}
	}
	
	IEnumerator LoadImage(string url)
	{
		//start download
		var www = new WWW(url);

		//wait until download completes
		yield return www; // wait for www to return

		//create a DXT1 (compressed) texture
		Texture2D texture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.DXT1, false);
		
		//load the image into a texture
		www.LoadImageIntoTexture(texture);
		Rect rect = new Rect(0, 0, texture.width, texture.height);
		
		//load the texture into a sprite.
		Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100);
		m_image.sprite = sprite;
		
		//set the image to native size
		m_image.SetNativeSize();

	}	
	
	

	public void OnClick()
	{
		StartCoroutine("Search");
	}
	
}
