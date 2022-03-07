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
    public GameObject[] playerPosition;

    [SerializeField]
    private List<ScriptableCard> deckCards;
    [SerializeField]
    private GameObject[] cardsPosition;

    public List<CardBehavior> boardCards = new List<CardBehavior>();

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
                PickupAliment(i);
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


    private void Update()
    {

    }

    public void PickupAliment(int emplacement)
    {
        CmdCreateCard(emplacement);
    }

    [Command(requiresAuthority = false)]
    private void CmdCreateCard(int emplacement)
    {
        GameObject cardObj = Instantiate(cardObject, deckObject.transform.position, Quaternion.identity);
        //cardObj.AddComponent<CardBehavior>();
        cardObj.GetComponent<CardBehavior>().InitializeCard(deckCards[0]);
        NetworkServer.Spawn(cardObj,netIdentity.connectionToClient);
        RpcCreateCard(cardObj,emplacement);
    }

    [ClientRpc]
    private void RpcCreateCard(GameObject cardObj, int emplacement)
    {
        Debug.Log(cardObj + " " + deckCards[0]);
        cardObj.transform.position = cardsPosition[emplacement].transform.position; //La position de la carte pioché étant, la taille de la main
        cardObj.transform.rotation = Quaternion.Euler(90, 0, 0);
        cardObj.GetComponent<CardBehavior>().InitializeCard(deckCards[0]);
        cardObj.GetComponent<CardBehavior>().emplacementFood = emplacement;
        boardCards.Add(cardObj.GetComponent<CardBehavior>());
        deckCards.RemoveAt(0);
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
        player.statePlayer = PlayerBehavior.StatePlayer.DrawPhase;
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
    public void CmdPickInReserve(CardBehavior card)
    {
        RpcPickInReserve(card);
        if (deckCards.Count > 0)
        {
            PickupAliment(card.emplacementFood);
        }
    }

    [ClientRpc]
    private void RpcPickInReserve(CardBehavior card)
    {
        boardCards.Remove(card);
    }


}
