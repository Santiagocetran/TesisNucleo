using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PressKeyOpenDoor : MonoBehaviour
{
    public GameObject Instruction;
    public GameObject AnimeObject;
    public GameObject AnimeObject2;
    public GameObject PasswordPopup;
    public TMP_InputField PasswordInputField;
    public Button SubmitButton;

    public MonoBehaviour firstPersonMovement; // Reference to the movement script
    public MonoBehaviour firstPersonLook;     // Reference to the look script

    private bool Action = false;
    private string correctPassword = "quimera";

    void Start()
    {
        Instruction.SetActive(false);
        PasswordPopup.SetActive(false);

        SubmitButton.onClick.AddListener(CheckPassword);
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag == "Player")
        {
            Instruction.SetActive(true);
            Action = true;
        }
    }

    void OnTriggerExit(Collider collision)
    {
        Action = false;
        PasswordPopup.SetActive(false);
        EnablePlayerControl(); // Re-enable control when leaving the area
        Instruction.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Action)
            {
                Instruction.SetActive(false);

                if (PasswordPopup == null)
                    Debug.LogError("PasswordPopup is not assigned.");
                else
                    PasswordPopup.SetActive(true);

                if (PasswordInputField == null)
                    Debug.LogError("PasswordInputField is not assigned.");
                else
                    PasswordInputField.ActivateInputField(); // Set focus on the input field

                DisablePlayerControl(); // Disable player movement and camera
            }
        }
    }


    void CheckPassword()
    {
        if (PasswordInputField.text == correctPassword)
        {
            // Correct password, open the door
            AnimeObject.GetComponent<Animator>().Play("BarreraLaser1");
            AnimeObject2.GetComponent<Animator>().Play("BarreraLaser2");
            PasswordPopup.SetActive(false);
            EnablePlayerControl(); // Re-enable player movement and camera
            Action = false;
        }
        else
        {
            PasswordInputField.text = ""; // Clear the input field
            // Optionally, show feedback for incorrect password
        }
    }

    private void DisablePlayerControl()
    {
        firstPersonMovement.enabled = false; // Disable movement script
        firstPersonLook.enabled = false;     // Disable look script
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Show the cursor
    }

    private void EnablePlayerControl()
    {
        if (firstPersonMovement == null)
            Debug.LogError("First Person Movement script is not assigned.");
        else
            firstPersonMovement.enabled = true;

        if (firstPersonLook == null)
            Debug.LogError("First Person Look script is not assigned.");
        else
            firstPersonLook.enabled = true;

        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        Cursor.visible = false; // Hide the cursor
    }

}
