using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DeckManager : NetworkBehaviour
{
    [SerializeField]
    private GameObject cardObject;
    [SerializeField]
    private GameObject deckObject;

    [SerializeField]
    private List<ScriptableCard> deckCards;
    [SerializeField]
    private GameObject[] cardsPosition;

    public List<CardBehavior> boardCards = new List<CardBehavior>();

    [SyncVar] public GameObject playerHost;
    [SyncVar] public GameObject playerClient;


    [SyncVar] public int tour = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
        {
           
        }

        int playerStart = Random.Range(0, 2);
        switch (playerStart)
        {
            case 0:
                break;
            case 1:
                break;
        }
    }

    private bool DoOnce = false;

    private void Update()
    {
        if (isServer)
        {
            //On attend que tous les joueurs se connectes
            if (!DoOnce)
            {
                if (!allPlayersReady())
                {

                }
                else
                {
                    for (int i = 0; i < cardsPosition.Length; i++)
                    {
                        PickupAliment(i);
                        DoOnce = true;
                    }

                    ChangeBoardAuthority(netIdentity.connectionToClient);
                }
            }
        }
    }

    public void PickupAliment(int emplacement)
    {
        CmdCreateCard(emplacement);
        deckCards.RemoveAt(0);
    }

    [Command(requiresAuthority = false)]
    private void CmdCreateCard(int emplacement)
    {
        GameObject cardObj = Instantiate(cardObject, deckObject.transform.position, Quaternion.identity);

        cardObj.GetComponent<CardBehavior>().InitializeCard(deckCards[0]);
        deckCards.RemoveAt(0);
        cardObj.transform.position = cardsPosition[emplacement].transform.position; //La position de la carte pioché étant, la taille de la main
        cardObj.transform.rotation = Quaternion.Euler(90, 0, 0);
        boardCards.Add(cardObj.GetComponent<CardBehavior>());

        NetworkServer.Spawn(cardObj);
        RpcCreateCard(cardObj);
    }

    [ClientRpc]
    private void RpcCreateCard(GameObject cardObj)
    {
        Debug.Log(cardObj + " " + deckCards[0]);
        cardObj.GetComponent<CardBehavior>().InitializeCard(deckCards[0]);
        boardCards.Add(cardObj.GetComponent<CardBehavior>());
        deckCards.RemoveAt(0);
    }

    public bool CanStartMatch()
    {
        if (playerHost == null)
            return false;
        if (playerClient == null)
            return false;



        return true;
    }

    [Command(requiresAuthority =false)]
    public void CmdNextTurn()
    {
        playerHost.GetComponent<PlayerBehavior>().yourTurn = !playerHost.GetComponent<PlayerBehavior>().yourTurn;
        if (playerHost.GetComponent<PlayerBehavior>().yourTurn)
        {
            ChangeBoardAuthority(playerHost.GetComponent<NetworkIdentity>().connectionToClient);
            Debug.Log("Host auto");
        }

        playerClient.GetComponent<PlayerBehavior>().yourTurn = !playerClient.GetComponent<PlayerBehavior>().yourTurn;

        if (playerClient.GetComponent<PlayerBehavior>().yourTurn)
        {
            ChangeBoardAuthority(playerClient.GetComponent<NetworkIdentity>().connectionToClient);
            Debug.Log("Client auto");
        }
    }

    //Permet de changer l'autorithé des cartes au milieu
    [Server]
    public void ChangeBoardAuthority(NetworkConnection conn)
    {
        foreach(CardBehavior cb in boardCards)
        {
            cb.gameObject.GetComponent<NetworkIdentity>().RemoveClientAuthority();
            cb.gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
        }
    }

    [ServerCallback]
    private bool allPlayersReady()
    {
        foreach(NetworkConnection nt in NetworkServer.connections.Values)
        {
            if (!nt.isReady)
            {
                Debug.Log(nt);
                return false;
            }
        }

        return true;
    }


}
