using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class MenuBehavior : MonoBehaviour
{
    #region Références
    public GameObject menuCanvas;
    public GameObject lobbyCanvas;
    public TMP_InputField ipText;
    #endregion

    void Start()
    {
        menuCanvas.SetActive(true);
        lobbyCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Boutton Host
    public void OnPressedHost()
    {
        menuCanvas.SetActive(false);
        lobbyCanvas.SetActive(true);
        NetworkManager.singleton.StartHost();
    }

    //Boutton Join

    public void OnPressedClient()
    {
        menuCanvas.SetActive(false);
        lobbyCanvas.SetActive(true);
        NetworkManager.singleton.networkAddress = ipText.text;
        NetworkManager.singleton.StartClient();
    }

    public void OnPressedDisconnect()
    {
        if (NetManager.isHost)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }

        menuCanvas.SetActive(true);
        lobbyCanvas.SetActive(false);

        NetManager.isHost = false;
    }


}
