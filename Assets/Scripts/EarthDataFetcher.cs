using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EarthDataFetcher : MonoBehaviour
{
    // Modify the URL to include a date range filter
    private string soilMoistureDataUrl = "https://cmr.earthdata.nasa.gov/search/granules.json?dataset_id=SMAP_L3_SM_P&temporal=2023-08-01T00:00:00Z,2023-08-31T23:59:59Z";
    private string username = "luqman_1234";  // Replace with your NASA EarthData username
    private string password = "G00lden@falc00n";  // Replace with your NASA EarthData password

    IEnumerator FetchSoilMoistureData()
    {
        UnityWebRequest request = UnityWebRequest.Get(soilMoistureDataUrl);

        // Encode the credentials and set the Authorization header
        string credentials = System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(username + ":" + password));
        request.SetRequestHeader("Authorization", "Basic " + credentials);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            // Output the JSON response from the NASA CMR API
            string jsonResult = request.downloadHandler.text;
            Debug.Log("Soil Moisture Data: " + jsonResult);
            // Optionally, you can parse the JSON and display the data
        }
    }

    void Start()
    {
        StartCoroutine(FetchSoilMoistureData());
    }
}
