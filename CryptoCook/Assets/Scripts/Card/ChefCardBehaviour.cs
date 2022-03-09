using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using static PlayerBehavior;

public class ChefCardBehaviour : CardBehavior
{
    public TextMeshPro textName;
    public TextMeshPro textCost;
    public TextMeshPro textCostFood;
    public TextMeshPro textRecipeType;
    public TextMeshPro textCulture;
    public TextMeshPro textDescription;
    public SpriteRenderer spriteBackgroud;

    public ChefCardScriptable cardLogic;

    public bool isOnBoard = false;

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

        if(cardLogic.cardType == ScriptableCard.CardType.Recette)
        {
            //Text Assignement
            textName.text = cardLogic.cardName;
            currentCost = new List<ChefCardScriptable.Cost>(cardLogic.cost);
            textCost.text = (basePoint + variablePoint).ToString();
            string foodCost = "";
            for (int i = 0; i < currentCost.Count; i++)
            {
                if (currentCost[i].costType == ChefCardScriptable.Cost.CostType.AlimentType)
                {
                    foodCost = currentCost[i].alimentTypeCost.ToString() + " + ";
                }


                if (currentCost[i].costType == ChefCardScriptable.Cost.CostType.Gout)
                {
                    foodCost = currentCost[i].goutCost.ToString() + " + ";
                }

                if (currentCost[i].costType == ChefCardScriptable.Cost.CostType.Specific)
                {
                    foodCost = currentCost[i].specificCost.cardName + " + ";
                }


            }
            textCostFood.text = foodCost;
            textRecipeType.text = cardLogic.recipeType.ToString();
            textCulture.text = cardLogic.recipeCulture.ToString();
            if(cardLogic.effect != null)
                textDescription.text = cardLogic.effect.effectDescription;
        }

        if(cardLogic.cardType == ScriptableCard.CardType.Effet)
        {

            if(cardLogic.sprite != null)
            {
                spriteBackgroud.sprite = cardLogic.sprite;
            }
            
            string foodCost = "";
            for (int i = 0; i < currentCost.Count; i++)
            {
                if (currentCost[i].costType == ChefCardScriptable.Cost.CostType.AlimentType)
                {
                    foodCost = currentCost[i].alimentTypeCost.ToString() + " + ";
                }


                if (currentCost[i].costType == ChefCardScriptable.Cost.CostType.Gout)
                {
                    foodCost = currentCost[i].goutCost.ToString() + " + ";
                }

                if (currentCost[i].costType == ChefCardScriptable.Cost.CostType.Specific)
                {
                    foodCost = currentCost[i].specificCost.cardName + " + ";
                }


            }
            textName.text = cardLogic.cardName;
            textCostFood.text = foodCost;
            if (cardLogic.effect != null)
                textDescription.text = cardLogic.effect.effectDescription;

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
        }
    }

    public override void OnMouseUp()
    {
        deckManager.dragPlane.SetActive(false);
        if (hasAuthority)
        {
            deckManager.authorityPlayer.isDraggingCard = false;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("DropCard")))
            {
                if (hit.transform.tag == "Board" && !isOnBoard)
                {
                    if (player.statePlayer == StatePlayer.PlayCardPhase)
                    {
                        if(player.CanPlayCard(this))
                        {
                            isOnBoard = true;
                            player.CmdDropCardOnBoard(this, player.FindBoardPlaces(hit.transform.gameObject));
                            player.UseEngagedAliment();
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

    public void ResetToHand()
    {
        GameObject camRef = deckManager.posCamP2;

        if(isClient)
        {
            camRef = deckManager.posCamP1;
        }

        ResetPos();
        float angle = Vector3.Angle(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(camRef.transform.position.x, 0, camRef.transform.position.z));
        if (transform.position.x <= camRef.transform.position.x)
        {
            angle = -angle;
        }
        transform.localRotation = Quaternion.Euler(50, angle, 0);
        
    }

    public void RefreshEffect()
    {
        variablePoint = 0;
        if(cardLogic.effect != null)
            StartCoroutine(cardLogic.effect.OnBoardChange(this));
    }

    public void DestroyCard()
    {
        player.DestroyCardFromBoard(this);
    }

    public void AddCost(ChefCardScriptable.Cost addedCost)
    {
        currentCost.Add(addedCost);
        // refersh affichage
    }
}
