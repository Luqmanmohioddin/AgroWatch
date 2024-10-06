using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class PrecipitationManager : MonoBehaviour
{
    public TMP_InputField cityInputField; // Input field for city name
    public TMP_Text precipitationText; // Text to display precipitation probability
    private string apiKey = "FHZ94KSS92N2K2LJ85BTEVMMF" +
        ""; // Replace with your Visual Crossing API key

    private void Start()
    {
        // Add listener for Enter key press
        cityInputField.onSubmit.AddListener(delegate { FetchPrecipitationData(); });
    }

    public void FetchPrecipitationData()
    {
        string cityName = cityInputField.text;
        precipitationText.text = $"Fetching precipitation for: {cityName}"; // Display a message
        GetWeatherData(cityName);
    }

    void GetWeatherData(string cityName)
    {
        string url = $"https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/{UnityWebRequest.EscapeURL(cityName)}?key={apiKey}";
        Debug.Log($"Weather Data Request URL: {url}");
        StartCoroutine(GetWeatherDataCoroutine(url));
    }

    IEnumerator GetWeatherDataCoroutine(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + webRequest.error);
                precipitationText.text = "Error retrieving weather data.";
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                ProcessWeatherData(jsonResponse);
            }
        }
    }

    void ProcessWeatherData(string json)
    {
        WeatherResponses weatherResponse = JsonUtility.FromJson<WeatherResponses>(json);

        // Assuming the precipitation probability is located in the "days" array
        if (weatherResponse.days.Length > 0)
        {
            float precipitationProbability = weatherResponse.days[0].precipprob; // Get the precipitation probability
            precipitationText.text = $"Precipitation Probability: {precipitationProbability}%";
        }
        else
        {
            precipitationText.text = "No weather data available.";
        }
    }
}

// WeatherResponses class to match the JSON structure
[System.Serializable]
public class WeatherResponses
{
    public Day[] days;

    [System.Serializable]
    public class Day
    {
        public float precipprob; // Precipitation probability
    }
}
