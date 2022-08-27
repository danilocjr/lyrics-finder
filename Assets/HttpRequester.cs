using System.Collections;
using System.Collections.Generic;
using System.Web;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HttpRequester : MonoBehaviour
{

    public Text title;
    public Text lyric;

    public string query;

    //HTML Parameters and Structure
    const string bgnParam = "class=\"BNeawe tAd8D AP7Wnd\">";
    const string endParam= "</div>";
    const string endParamTitle = "</span>";
    const int titlePos = 0;
    const int lyricPos = 4;
    const string titleKey = "title";
    const string lyricKey = "lyric";


    void Start()
    {
        string url = string.Format("https://www.google.com/search?q={0}+lyrics", HttpUtility.UrlEncode(query));
        //Debug.Log(string.Format("URL:{0}", url));

        StartCoroutine(HttpGet(url));
    }
       
    public IEnumerator HttpGet(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(string.Format("Error[{0}]:{1}", webRequest.responseCode, webRequest.error));
            }
            else if (webRequest.isDone)
            {
                string data = System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data);
                Debug.Log(string.Format("Success[{0}]:{1}", webRequest.responseCode, webRequest.error));
                //Debug.Log(string.Format("DATA:{0}", data));
                if (!string.IsNullOrEmpty(data))
                {
                    PublishResults(DataParser(data));
                }                    
            }
        }
    }

    public void PublishResults(Dictionary<string, string> song)
    {
        title.text = song[titleKey];
        lyric.text = song[lyricKey];
    }

    class Excertp
    {
        public string content;
        public int startIndex;
        public int endIndex;
        public string text;
    };

    public Dictionary<string,string> DataParser(string data)
    {
        int startIndex = 0;
        int attempts = 10;

        List<Excertp> excertps = new List<Excertp>();

        //Find index for the start string
        for (int k = 0; k < attempts; k++)
        {
            int id = data.IndexOf(bgnParam, startIndex, System.StringComparison.Ordinal);
            if (id > startIndex)
            {
                if(k == titlePos)
                {
                    excertps.Add(new Excertp
                    {
                        content = "title",
                        startIndex = id + bgnParam.Length,
                        endIndex = 0,
                        text = ""
                    }) ;
                }
                else if (k == lyricPos)
                {
                    excertps.Add(new Excertp
                    {
                        content = "lyric",
                        startIndex = id + bgnParam.Length,
                        endIndex = 0,
                        text = ""
                    });
                    break;
                }
                startIndex = id + 1;

            }
            else
                break;
           
        }
        //Find index for the end string
        foreach (Excertp ex in excertps)
        {
            int id;
            if (ex.content == "title")
            {
                id = data.IndexOf(endParamTitle, ex.startIndex, System.StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                id = data.IndexOf(endParam, ex.startIndex, System.StringComparison.OrdinalIgnoreCase);
            }
            
            if (id > ex.startIndex)
            {
                ex.endIndex = id;
                ex.text = data.Substring(ex.startIndex, ex.endIndex - ex.startIndex);
                //Debug.Log(string.Format("[{0}] {1}", ex.content, ex.text));
            }
            else
            {
                break;
            }
        }

        //Return results
        Dictionary<string, string> song = new Dictionary<string, string>();
        foreach (Excertp ex in excertps)
            song[ex.content] = ex.text;

        return song;

    }
}
