using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Text;

public class GeminiTest : MonoBehaviour
{
    [SerializeField] private string apiKey = "API KEY";
    [SerializeField] private string testPrompt = "Write a short poem about AI";
    
    private const string API_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

    void Start()
    {
        StartCoroutine(TestGeminiAPI());
    }

    IEnumerator TestGeminiAPI()
    {
        string url = $"{API_URL}?key={apiKey}";
        
        // Create request body
        string jsonBody = $@"
        {{
            ""contents"": [{{
                ""parts"": [{{
                    ""text"": ""{testPrompt}""
                }}]
            }}]
        }}";

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✓ Gemini API Response:");
                Debug.Log(request.downloadHandler.text);
                
                // Parse the response to get just the text
                ParseResponse(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"✗ Error: {request.error}");
                Debug.LogError($"Response: {request.downloadHandler.text}");
            }
        }
    }
    
    void ParseResponse(string jsonResponse)
    {
        // Simple parsing to extract the generated text
        try
        {
            int textIndex = jsonResponse.IndexOf("\"text\": \"") + 9;
            int endIndex = jsonResponse.IndexOf("\"", textIndex);
            string generatedText = jsonResponse.Substring(textIndex, endIndex - textIndex);
            
            // Unescape newlines
            generatedText = generatedText.Replace("\\n", "\n");
            
            Debug.Log("\n--- Generated Text ---");
            Debug.Log(generatedText);
            Debug.Log("----------------------\n");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Could not parse response: {e.Message}");
        }
    }

    void Update()
    {
        
    }
}
