using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using TMPro;

[System.Serializable]
public class WeatherResponse
{
    public List<Forecast> list;
}

[System.Serializable]
public class Forecast
{
    public long dt; // Date in UNIX timestamp
    public Main main;
    public List<Weather> weather;
    public Wind wind;
    public Clouds clouds;
    public int visibility;
}

[System.Serializable]
public class Main
{
    public float temp;
    public float feels_like;
    public float temp_min;
    public float temp_max;
    public float pressure;
    public int humidity;
}

[System.Serializable]
public class Weather
{
    public string description;
}

[System.Serializable]
public class Wind
{
    public float speed;
    public int deg;
}

[System.Serializable]
public class Clouds
{
    public int all;
}

public class WeatherFetcher : MonoBehaviour
{
    public TMP_Text cityNameText, dateText, temperatureText, feelsLikeText, tempMinText, tempMaxText, humidityText, descriptionText, windSpeedText, windDirectionText, cloudinessText, visibilityText, pressureText;
    public TMP_InputField cityInputField; // Input field for city

    private string apiKey = "898e6c47a492c003929948e5e88a7453"; // Replace with your API key
    private string baseURL = "https://api.openweathermap.org/data/2.5/forecast";

    void Start()
    {
        cityInputField.onEndEdit.AddListener(OnCityEntered); // Listen for the Enter key in the input field
    }

    // Called when the user presses Enter in the input field
    void OnCityEntered(string city)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (!string.IsNullOrEmpty(city))
            {
                StartCoroutine(GetWeatherData(city)); // Fetch weather data for the specified city
            }
        }
    }

    IEnumerator GetWeatherData(string city)
    {
        string url = $"{baseURL}?q={city}&appid={apiKey}";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {webRequest.error}");
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log(jsonResponse);
                ParseWeatherData(jsonResponse, city);
            }
        }
    }

    void ParseWeatherData(string json, string city)
    {
        WeatherResponse weatherResponse = JsonUtility.FromJson<WeatherResponse>(json);
        var forecast = weatherResponse.list[0]; // Get the first forecast for simplicity

        DateTime date = DateTimeOffset.FromUnixTimeSeconds(forecast.dt).DateTime;
        float tempCelsius = forecast.main.temp - 273.15f;
        float feelsLikeCelsius = forecast.main.feels_like - 273.15f;
        float tempMinCelsius = forecast.main.temp_min - 273.15f;
        float tempMaxCelsius = forecast.main.temp_max - 273.15f;
        float windSpeed = forecast.wind.speed;
        string windCompassDirection = GetCompassDirection(forecast.wind.deg);
        int cloudiness = forecast.clouds.all;
        int visibility = forecast.visibility / 1000;
        float pressure = forecast.main.pressure;
        int humidity = forecast.main.humidity;

        // Assign the data to respective UI elements
        cityNameText.text = $"City: {city}";
        dateText.text = $"Date: {date}";
        temperatureText.text = $"{tempCelsius:F1}°C";
        feelsLikeText.text = $"Feels Like: {feelsLikeCelsius:F1}°C";
        tempMinText.text = $"Min Temp: {tempMinCelsius:F1}°C";
        tempMaxText.text = $"Max Temp: {tempMaxCelsius:F1}°C";
        humidityText.text = $"Humidity: {humidity}%";
        descriptionText.text = $"Description: {forecast.weather[0].description}";
        windSpeedText.text = $"Wind Speed: {windSpeed:F1} m/s";
        windDirectionText.text = $"Wind Direction: {windCompassDirection}";
        cloudinessText.text = $"Cloudiness: {cloudiness}%";
        visibilityText.text = $"Visibility: {visibility} km";
        pressureText.text = $"Pressure: {pressure} hPa";
    }

    // Helper function to convert degrees to compass points
    string GetCompassDirection(int degrees)
    {
        string[] compassPoints = { "North", "North-East", "East", "South-East", "South", "South-West", "West", "North-West" };
        int index = Mathf.RoundToInt(degrees / 45f) % 8;
        return compassPoints[index];
    }
}
