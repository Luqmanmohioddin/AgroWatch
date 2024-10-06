using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using UnityEngine.UI; // Include the UI namespace for Slider
using System.Collections.Generic; // For List<T>

public class AirQualityManager : MonoBehaviour
{
    public TMP_InputField cityInputField; // Input field for city name
    public TMP_Text aqiDescriptionText; // Text for AQI description
    public TMP_Text aqiValueText; // Text for AQI value
    //public Slider aqiSlider; // Slider for AQI value
    public Image sliderFillImage; // Image for the slider fill color
    public TMP_Text coText; // Text for CO value
    public TMP_Text noText; // Text for NO value
    public TMP_Text no2Text; // Text for NO2 value
    public TMP_Text o3Text; // Text for O3 value
    public TMP_Text so2Text; // Text for SO2 value
    public TMP_Text pm25Text; // Text for PM2.5 value
    public TMP_Text pm10Text; // Text for PM10 value
    public TMP_Text nh3Text; // Text for NH3 value

    private string apiKey = "898e6c47a492c003929948e5e88a7453"; // Replace with your OpenWeatherMap API key

    private void Start()
    {
        // Add listener to handle Enter key press on the input field
        cityInputField.onEndEdit.AddListener(OnEnterPressed);
    }

    private void OnEnterPressed(string cityName)
    {
        if (!string.IsNullOrWhiteSpace(cityName))
        {
            GetCoordinates(cityName);
        }
    }

    void GetCoordinates(string cityName)
    {
        string url = $"https://api.openweathermap.org/data/2.5/weather?q={UnityWebRequest.EscapeURL(cityName)}&appid={apiKey}";
        Debug.Log($"Geocoding Request URL: {url}");
        StartCoroutine(GetCityCoordinates(url));
    }

    IEnumerator GetCityCoordinates(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + webRequest.error);
                aqiDescriptionText.text = "Error retrieving location data.";
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                ProcessCityCoordinates(jsonResponse);
            }
        }
    }

    void ProcessCityCoordinates(string json)
    {
        CityResponse cityData = JsonUtility.FromJson<CityResponse>(json);

        if (cityData.coord != null)
        {
            float latitude = cityData.coord.lat;
            float longitude = cityData.coord.lon;
            Debug.Log($"Coordinates: {latitude}, {longitude}");
            GetAirQuality(latitude, longitude);
        }
        else
        {
            aqiDescriptionText.text = "City not found.";
        }
    }

    void GetAirQuality(float latitude, float longitude)
    {
        string url = $"https://api.openweathermap.org/data/2.5/air_pollution?lat={latitude}&lon={longitude}&appid={apiKey}";
        Debug.Log($"Air Quality Request URL: {url}");
        StartCoroutine(GetAirQualityData(url));
    }

    IEnumerator GetAirQualityData(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + webRequest.error);
                aqiDescriptionText.text = "Error retrieving air quality data.";
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                ProcessAirQualityData(jsonResponse);
            }
        }
    }

    void ProcessAirQualityData(string json)
    {
        AirQualityResponse airQualityData = JsonUtility.FromJson<AirQualityResponse>(json);

        if (airQualityData.list.Length > 0)
        {
            int aqiValue = airQualityData.list[0].main.aqi;
            aqiValueText.text = $"AQI Value: {aqiValue}"; // Display AQI value
            //aqiSlider.value = aqiValue; // Update slider value

            // Set AQI description based on the value
            string aqiDescription = aqiValue switch
            {
                1 => "Good",
                2 => "Fair",
                3 => "Moderate",
                4 => "Poor",
                5 => "Very Poor",
                _ => "Unknown"
            };

            aqiDescriptionText.text = $"{aqiDescription}";

            // Change the slider fill color based on AQI value
            if (aqiValue <= 2)
            {
                sliderFillImage.color = Color.green; // Good
            }
            else if (aqiValue <= 3)
            {
                sliderFillImage.color = Color.yellow; // Moderate
            }
            else
            {
                sliderFillImage.color = Color.red; // Poor and Very Poor
            }

            // Display other air quality parameters
            coText.text = $"CO: {airQualityData.list[0].components.co} µg/m³";
            noText.text = $"NO: {airQualityData.list[0].components.no} µg/m³";
            no2Text.text = $"NO2: {airQualityData.list[0].components.no2} µg/m³";
            o3Text.text = $"O3: {airQualityData.list[0].components.o3} µg/m³";
            so2Text.text = $"SO2: {airQualityData.list[0].components.so2} µg/m³";
            pm25Text.text = $"PM2.5: {airQualityData.list[0].components.pm2_5} µg/m³";
            pm10Text.text = $"PM10: {airQualityData.list[0].components.pm10} µg/m³";
            nh3Text.text = $"NH3: {airQualityData.list[0].components.nh3} µg/m³";
        }
        else
        {
            aqiDescriptionText.text = "No air quality data available.";
        }
    }
}

// Classes to match the JSON structure
[System.Serializable]
public class CityResponse
{
    public Coordinates coord;

    [System.Serializable]
    public class Coordinates
    {
        public float lat; // Latitude
        public float lon; // Longitude
    }
}

[System.Serializable]
public class AirQualityResponse
{
    public List[] list; // Should be List[]

    [System.Serializable]
    public class List
    {
        public Main main;
        public Components components;

        [System.Serializable]
        public class Main
        {
            public int aqi; // Air Quality Index
        }

        [System.Serializable]
        public class Components
        {
            public float co; // Carbon monoxide
            public float no; // Nitric oxide
            public float no2; // Nitrogen dioxide
            public float o3; // Ozone
            public float so2; // Sulfur dioxide
            public float pm2_5; // Particulate matter 2.5
            public float pm10; // Particulate matter 10
            public float nh3; // Ammonia
        }
    }
}
