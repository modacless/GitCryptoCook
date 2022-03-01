using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class LobbyPlayerBehavior : NetworkBehaviour
{
    [Header("Références")]
    #region Références
    public TextMeshProUGUI pseudoText;

    public GameObject[] lobbyPosition; //Position du joueur dans le lobby

    public GameObject readyUi;
    #endregion

    #region paramètres
    [Header("SyncVar")]
    [SyncVar(hook = nameof(UpdatePseudoText))]
    public string pseudo;
    [SyncVar(hook = nameof(UpdatePosition))]
    public int positionInLobby;

    [HideInInspector]
    [SyncVar(hook = nameof(UpdateReady))]
    public bool isReady;
    #endregion

    // Start is called before the first frame update

    private void Awake()
    {
        lobbyPosition = GameObject.FindGameObjectsWithTag("LobbyPosition");
    }
    void Start()
    {
        //Initialise toutes les valeurs syncs
        UpdatePosition(positionInLobby, positionInLobby);
        UpdatePseudoText(pseudo, pseudo);
        UpdateReady(isReady, isReady);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdatePosition(int oldValue, int newValue)
    {
        transform.SetParent(lobbyPosition[newValue].transform);
        Debug.Log("value + " + positionInLobby);
        GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
    }

    private void UpdatePseudoText(string oldText, string newText)
    {
        pseudoText.text = newText;
    }

    private void UpdateReady(bool oldVal, bool newValue)
    {
        readyUi.SetActive(newValue);
    }
    [Command]
    public void OnPressedReady()
    {
        isReady = !isReady;
    }


}
