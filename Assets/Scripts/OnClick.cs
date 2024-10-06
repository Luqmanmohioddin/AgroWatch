using UnityEngine;
using UnityEngine.EventSystems;

public class OnClick : MonoBehaviour
{
    public GameObject textObject; // Reference to the GameObject to enable/disable

    // Method to call when the button is clicked
    public void OnButtonClick()
    {
        // Activate the textObject
        textObject.SetActive(true);
    }

    void Update()
    {
        // Check if the Escape key is pressed
        if (textObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            textObject.SetActive(false); // Deactivate the textObject
        }
    }
}
