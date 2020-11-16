using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Watson Text To Speechを利用し音声合成をリクエストする
/// </summary>
public class GenerateAudio : MonoBehaviour
{
    [SerializeField] private string apiKey;
    [SerializeField] private string url;
    private const string VOICE = "ja-JP_EmiVoice";
    
    private const string AUTH_URL = "https://iam.cloud.ibm.com/identity/token";
    
    private string _authorizationKey;
    private bool _cancelled = false;

    /// <summary>
    /// APIキーからアクセストークンを取得
    /// </summary>
    public void Init()
    {
        Debug.Log("[GenerateAudio] Watson authorization started.");
        var form = new WWWForm();
        form.AddField("grant_type", "urn:ibm:params:oauth:grant-type:apikey");
        form.AddField("apikey", apiKey);
        form.AddField("response_type", "cloud_iam");
        using (var request = UnityWebRequest.Post(AUTH_URL, form))
        {
            request.SetRequestHeader("Content-type", "application/x-www-form-urlencoded");
            request.SendWebRequest();
            while(!request.isDone && !_cancelled){}

            if (request.responseCode != 200L)
            {
                Debug.LogError($"[GenerateAudio] Request Failed ({request.responseCode}): {request.error}\nat{request.url}");
                return;
            }

            var json = request.downloadHandler.text;
            _authorizationKey = JsonConvert.DeserializeObject<IamTokenResponse>(json).AccessToken;
        }
        Debug.Log("[GenerateAudio] Watson authorization succeeded.");
    }

    /// <summary>
    /// 音声合成リクエストを送信し，バイナリの合成結果を返す
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public byte[] GenerateBinary(string text)
    {
        Debug.Log($"[GenerateAudio] Send text: {text}");
        var rqStr = JsonConvert.SerializeObject(new JObject {["text"] = text});
        var url = $"{this.url}/v1/synthesize?voice={VOICE}";
        using (var request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(rqStr));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "audio/wav");
            request.SetRequestHeader("Authorization", $"Bearer {_authorizationKey}");
            request.SendWebRequest();
            while(!request.isDone && !_cancelled){}
            
            if (request.responseCode != 200L)
            {
                Debug.LogError($"[GenerateAudio] Request Failed ({request.responseCode}): {request.error}\nat{request.url}");
                return null;
            }
            
            Debug.Log("[GenerateAudio] Get audio binary succeeded.");
            return request.downloadHandler.data;
        }
    }
}