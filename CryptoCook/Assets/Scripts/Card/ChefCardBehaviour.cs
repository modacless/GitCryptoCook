using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using static PlayerBehavior;

public class ChefCardBehaviour : CardBehavior
{
    public TextMeshPro textName;
    public TextMeshPro scoreText;
    public TextMeshPro costText;
    public TextMeshPro textRecipeType;
    public TextMeshPro textDescription;

    public MeshRenderer graph;

    public GameObject modelParent;

    public MeshRenderer cylindreBackground;

    public ChefCardScriptable cardLogic;

    public SpriteRenderer spriteRenderer;

    public MeshRenderer effectBackground;

    [SyncVar] public bool isOnBoard = false;

    public Repas repas;
    public int basePoint;
    public int variablePoint;

    public bool isEffectActive;

    [HideInInspector]
    [SyncVar] public int emplacementHand = -1;

    public List<ChefCardScriptable.Cost> currentCost;

    public void InitializeCard(ChefCardScriptable card, PlayerBehavior _player)
    {

        isEffectActive = true;
        player = _player;
        cardLogic = card;
        basePoint = cardLogic.pointEarn;



        if (cardLogic.cardType == ScriptableCard.CardType.Recette)
        {
            textName.text = cardLogic.cardName;
            currentCost = new List<ChefCardScriptable.Cost>(cardLogic.cost);
            scoreText.text = (basePoint + variablePoint).ToString();

            RefreshCostDisplay();

            textRecipeType.text = cardLogic.recipeType.ToString() + " | " + cardLogic.recipeCulture.ToString();
            if (cardLogic.effect != null)
                textDescription.text = cardLogic.effect.effectDescription;
            if (cardLogic.modelPrefab != null)
            {
                GameObject go = Instantiate(cardLogic.modelPrefab).gameObject;
                go.transform.SetParent(modelParent.transform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = new Vector3(1, 1, 0.01f);
                go.transform.localRotation = Quaternion.Euler(Vector3.zero);
            }

            Material[] matArray = graph.materials;

            if (cardLogic.matRarity != null)
                matArray[4] = cardLogic.matRarity;


            graph.materials = matArray;

            if (cardLogic.effectBackground != null && cylindreBackground != null)
            {
                cylindreBackground.GetComponent<MeshRenderer>().material = cardLogic.effectBackground;
            }

            if (cardLogic.effectBackground != null)
            {
                effectBackground.GetComponent<MeshRenderer>().material = cardLogic.effectBackground;
            }
        }

        if(cardLogic.cardType == ScriptableCard.CardType.Effect)
        {
            currentCost = new List<ChefCardScriptable.Cost>(cardLogic.cost);

            RefreshCostDisplay();

            textName.text = cardLogic.cardName;

            if (cardLogic.effect != null)
                textDescription.text = cardLogic.effect.effectDescription;

            Material[] matArray = graph.materials;

            if (cardLogic.spriteBackground != null)
                matArray[2] = cardLogic.spriteBackground;

            if(cardLogic.effectIllu != null)
            {
                spriteRenderer.sprite = cardLogic.effectIllu;
            }

            graph.materials = matArray;

            if(cardLogic.effectBackground != null)
            {
                effectBackground.GetComponent<MeshRenderer>().material = cardLogic.effectBackground;
            }
        }
        

    }

    public override void OnMouseDrag()
{
    if (hasAuthority && !isOnBoard && !deckManager.authorityPlayer.cardIsZoom)
    {
        //Vector3 ScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, mZCoord);
        //Vector3 newWorldPosition = Camera.main.ScreenToWorldPoint(ScreenPosition);
        //transform.position = newWorldPosition;
        deckManager.authorityPlayer.isDraggingCard = true;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("DragPlane")))
        {
            transform.position = hit.point;
        }

        if (Physics.Raycast(transform.position + Vector3.up * 20f, Vector3.down, out hit, 200f, LayerMask.GetMask("DropCard")))
        {
            if (hit.transform.tag == "Board")
            {
                if (player.statePlayer == StatePlayer.PlayCardPhase)
                {
                    player.HighlightRepas(player.FindBoardPlaces(hit.transform.gameObject), true);
                }
            }
        }
        else
        {
            player.LowLightAllRepas();
        }
    }
    }

    public override void OnMouseUp()
    {
        deckManager.dragPlane.SetActive(false);
        if (hasAuthority)
        {
            player.LowLightAllRepas();
            deckManager.authorityPlayer.isDraggingCard = false;
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 20f, Vector3.down, out hit, 200f, LayerMask.GetMask("DropCard")))
            {
                if (hit.transform.tag == "Board" && !isOnBoard)
                {
                    if (player.statePlayer == StatePlayer.PlayCardPhase)
                    {
                        if(player.CanPlayCard(this))
                        {
                            CmdPutOnBoard(true);
                            player.CmdDropCardOnBoard(this, player.FindBoardPlaces(hit.transform.gameObject));
                            player.UseEngagedAliment();

                            AudioManager.AMInstance.PlaySFX(AudioManager.AMInstance.dropCardSFX, 1.2f);
                        }
                        else
                        {
                            ResetToHand();
                        }
                    }
                    else
                    {
                        ResetToHand();
                    }

                }
                else
                {
                    ResetToHand();
                }
            }
            else
            {
                ResetToHand();
            }
        }
    }

    [Command]
    public void CmdPutOnBoard(bool isOn)
    {
        isOnBoard = isOn;
    }

    public void ResetToHand()
    {
        ResetPos();
    }

    public void RefreshEffect()
    {
        variablePoint = 0;
        if(cardLogic.effect != null && isEffectActive)
            StartCoroutine(cardLogic.effect.OnBoardChange(this));
    }

    public void DestroyCard()
    {
        player.CmdDestroyCardFromBoard(this);
    }

    public void RefreshCostDisplay()
    {
        string cotsTextString = "";
        for (int i = 0; i < currentCost.Count; i++)
        {
            if (currentCost[i].costType == ChefCardScriptable.Cost.CostType.AlimentType)
            {
                cotsTextString += currentCost[i].alimentTypeCost.ToString();
            }

            if (currentCost[i].costType == ChefCardScriptable.Cost.CostType.Gout)
            {
                cotsTextString += currentCost[i].goutCost.ToString();
            }

            if (currentCost[i].costType == ChefCardScriptable.Cost.CostType.Specific)
            {
                cotsTextString += currentCost[i].specificCost.cardName;
            }

            if (i < currentCost.Count - 1)
            {
                cotsTextString += " + ";
            }
        }

        costText.text = cotsTextString;
    }

    public void RefreshScore()
    {
        scoreText.text = (basePoint + variablePoint).ToString();
    }

    [Command(requiresAuthority = false)]
    public void CmdAddCost(string addedCost)
    {
        RpcAddCost(addedCost);
    }

    public ChefCardScriptable.Cost pouletCost;

    [ClientRpc]
    public void RpcAddCost(string addedCost)
    {
        if (addedCost == "Poulet")
        {
            AddCost(pouletCost);
        }
        else
        {
            Debug.Log("Please hardcode the cost here, can't send the cost in the network directly (oblky string :'( )");
        }
    }

    public void AddCost(ChefCardScriptable.Cost addedCost)
    {
        currentCost.Add(addedCost);
        RefreshCostDisplay();
    }


    [Command(requiresAuthority = false)]
    public void CmdDisabelEffect()
    {
        RpcDisableEffect();
    }

    [ClientRpc]
    public void RpcDisableEffect()
    {
        isEffectActive = false;
    }
}
