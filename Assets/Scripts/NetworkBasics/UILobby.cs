using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILobby : MonoBehaviour
{
    [SerializeField] InputField joinRoomInput;
    [SerializeField] Button joinButton;
    [SerializeField] Button hostButton;

    public void Host()
    {
        DisableInteractivity();
    }

    public void Join()
    {
        DisableInteractivity();
    }

    void DisableInteractivity()
    {
        joinRoomInput.interactable = false;
        joinButton.interactable = false;
        hostButton.interactable = false;
    }
}
