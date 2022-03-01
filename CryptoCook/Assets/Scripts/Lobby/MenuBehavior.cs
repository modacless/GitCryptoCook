using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class MenuBehavior : MonoBehaviour
{
    #region Références
    public GameObject menuCanvas;
    public TMP_InputField ipText;
    #endregion

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Boutton Host
    public void OnPressedHost()
    {
        menuCanvas.SetActive(false);
        NetworkManager.singleton.StartHost();
    }

    //Boutton Join

    public void OnPressedClient()
    {
        menuCanvas.SetActive(false);
        NetworkManager.singleton.networkAddress = ipText.text;
        NetworkManager.singleton.StartClient();
    }


}
