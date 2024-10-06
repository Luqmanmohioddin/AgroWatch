using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SimpleFetcher : MonoBehaviour
{
    private string token = "eyJ0eXAiOiJKV1QiLCJvcmlnaW4iOiJFYXJ0aGRhdGEgTG9naW4iLCJzaWciOiJlZGxqd3RwdWJrZXlfb3BzIiwiYWxnIjoiUlMyNTYifQ.eyJ0eXBlIjoiVXNlciIsInVpZCI6Imx1cW1hbl8xMjM0IiwiZXhwIjoxNzMyMjAxODk0LCJpYXQiOjE3MjcwMTc4OTQsImlzcyI6Imh0dHBzOi8vdXJzLmVhcnRoZGF0YS5uYXNhLmdvdiJ9.uD4p9NTNkz5BvBiMX5UBknA6qPyWsxk_g_W_rmbRVZnt9Tc0cO0BN9xVC0pZAmLbmhDX8WZqOlsCPdbOwszkK7YIo3QsfLF7vfYhUuq8i1fN6ATBPSR7oOnDwZVN-rWK2gIkPCdladkXPEhmebW7fu58rfGKWGFFdDyd_WKQcowygX07HmPzM-b84mF57nqalr9Y2UmHtFh6KTQalQYn-PSQY_YtxT3W6JlNY9rjAVF2LdUeN-FQggujmQz5zyPP5crgHogDZSxstDOo9ZJJwUhIH7_RJZZaL4A_-R0CPbpIRERwsogHp4XaUutjNzlsQAqnhY8gdDqEs_sqyJBDRg";  // Replace with your generated NASA EarthData token

    // Test with a simple GPM dataset query
    private string testDataUrl = "https://cmr.earthdata.nasa.gov/search/granules.json?dataset_id=GPM_3IMERGDF&page_size=10";

    IEnumerator FetchData()
    {
        UnityWebRequest request = UnityWebRequest.Get(testDataUrl);
        request.SetRequestHeader("Authorization", "Bearer " + token);

        Debug.Log("Request URL: " + testDataUrl);
        Debug.Log("Authorization Token: " + token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            string jsonResult = request.downloadHandler.text;
            Debug.Log("Response: " + jsonResult);
        }
    }

    void Start()
    {
        StartCoroutine(FetchData());
    }
}
