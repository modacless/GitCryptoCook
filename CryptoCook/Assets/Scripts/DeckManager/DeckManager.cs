using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DeckManager : NetworkBehaviour
{
    public LayerMask cardMask;


    public GameObject dragPlane;
    [SerializeField]
    private GameObject cardObject;
    [SerializeField]
    private GameObject alimentObject;
    [SerializeField]
    private GameObject deckObject;
    [SerializeField]
    public GameObject[] playerPosition;

    [SerializeField]
    private List<AlimentScriptable> alimentDeck;
    [SerializeField]
    private GameObject[] cardsPosition;

    public List<CardBehavior> boardCards = new List<CardBehavior>(); // for network purpose
    public List<AlimentBehavior> tableAliments = new List<AlimentBehavior>();

    [SyncVar] public GameObject playerHost;
    [SyncVar] public GameObject playerClient;


    [SyncVar] public int tour = 0;


    // Start is called before the first frame update
    IEnumerator Start()
    {
        if (isServer)
        {
            yield return new WaitUntil(() => allPlayersReady());

            for (int i = 0; i < cardsPosition.Length; i++)
            {
                DrawAlimentToTable(i);
            }

            yield return new WaitUntil(() => playerHost != null);
            ChangeBoardAuthority(netIdentity.connectionToClient);
        }
            

        int playerStart = Random.Range(0, 2);
        switch (playerStart)
        {
            case 0:
                break;
            case 1:
                break;
        }
        yield return null;
    }

    public void DrawAlimentToTable(int emplacement)
    {
        CmdCreateCard(emplacement);
    }

    [Command(requiresAuthority = false)]
    private void CmdCreateCard(int emplacement)
    {
        GameObject newAlimentObject = Instantiate(alimentObject, deckObject.transform.position, Quaternion.identity);
        newAlimentObject.GetComponent<AlimentBehavior>().InitializeCard(alimentDeck[0]);
        NetworkServer.Spawn(newAlimentObject,netIdentity.connectionToClient);
        RpcCreateCard(newAlimentObject, emplacement);
    }

    [ClientRpc]
    private void RpcCreateCard(GameObject alimentObject, int emplacement)
    {
        //Debug.Log(alimentObject + " " + alimentDeck[0]);
        alimentObject.transform.position = cardsPosition[emplacement].transform.position; //La position de la carte pioché étant, la taille de la main > nope c'est pas la main ici, c'est la table
        alimentObject.transform.rotation = Quaternion.Euler(90, 0, 0);
        alimentObject.GetComponent<AlimentBehavior>().InitializeCard(alimentDeck[0]);
        alimentObject.GetComponent<AlimentBehavior>().emplacementFood = emplacement;
        tableAliments.Add(alimentObject.GetComponent<AlimentBehavior>());
        boardCards.Add(alimentObject.GetComponent<CardBehavior>());
        alimentObject.GetComponent<AlimentBehavior>().SetCurrentPosAsBase();
        alimentDeck.RemoveAt(0);
    }

    [Command(requiresAuthority = false)]
    public void CmdNextTurn()
    {
        playerHost.GetComponent<PlayerBehavior>().yourTurn = !playerHost.GetComponent<PlayerBehavior>().yourTurn;
        if (playerHost.GetComponent<PlayerBehavior>().yourTurn)
        {
            ChangeBoardAuthority(playerHost.GetComponent<NetworkIdentity>().connectionToClient);
            RpcNextTurn(playerHost.GetComponent<PlayerBehavior>());
            Debug.Log("Host auto");
        }

        playerClient.GetComponent<PlayerBehavior>().yourTurn = !playerClient.GetComponent<PlayerBehavior>().yourTurn;
        if (playerClient.GetComponent<PlayerBehavior>().yourTurn)
        {
            ChangeBoardAuthority(playerClient.GetComponent<NetworkIdentity>().connectionToClient);
            RpcNextTurn(playerClient.GetComponent<PlayerBehavior>());
            Debug.Log("Client auto");
        }
    }

    [ClientRpc]
    public void RpcNextTurn(PlayerBehavior player)
    {
        player.NewTurn();
    }

    //Permet de changer l'autorithé des cartes au milieu
    [Server]
    public void ChangeBoardAuthority(NetworkConnection conn)
    {
        foreach(CardBehavior cb in boardCards)
        {
            if(cb.gameObject.GetComponent<NetworkIdentity>().connectionToClient != null)
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
                return false;
            }
        }

        return true;
    }

    [Command(requiresAuthority = false)]
    public void CmdPickOnTable(AlimentBehavior aliment)
    {
        RpcPickOnTable(aliment);
        if (alimentDeck.Count > 0)
        {
            DrawAlimentToTable(aliment.emplacementFood);
        }
    }

    [ClientRpc]
    private void RpcPickOnTable(CardBehavior card)
    {
        boardCards.Remove(card);
        tableAliments.Remove((AlimentBehavior)card);

        UpdateAlimentCardHalo(false);
    }

    public void UpdateAlimentCardHalo(bool _state)
    {
        for(int i = 0; i < tableAliments.Count; i++)
        {
            tableAliments[i].cardHalo.SetActive(_state);
        }
    }

    
}
