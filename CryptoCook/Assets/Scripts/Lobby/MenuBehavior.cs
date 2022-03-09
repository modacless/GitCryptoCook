using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class MenuBehavior : MonoBehaviour
{
    #region Références
    public GameObject menuCanvas;
    #endregion

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPressedHost()
    {
        menuCanvas.SetActive(false);
    }

    public void OnPressedClient()
    {
        menuCanvas.SetActive(false);
    }


}
