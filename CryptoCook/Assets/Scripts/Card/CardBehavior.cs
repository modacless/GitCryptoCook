using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using static ScriptableCard;
using static PlayerBehavior;
using UnityEngine;
using Mirror;


public abstract class CardBehavior : NetworkBehaviour
{
    public LayerMask cardMask;

    #region r�f�rences
    public DeckManager deckManager;

    public GameObject cardHalo;
    #endregion

    #region Drag parameters
    protected float mZCoord;
    protected Vector3 basePosition;
    #endregion

    #region zoom
    bool cardIsZoom = false;
    GameObject zoomedCard = null;
    Vector3 positionBeforeZoom;
    Quaternion rotationBeforeZoom;
    #endregion

    public PlayerBehavior player;
    private StatePlayer phaseBeforeZoom;
    private UnityEvent OnUse;

    

    void Start()
    {
        deckManager = GameObject.Find("GameManager").GetComponent<DeckManager>();
        mZCoord = Camera.main.WorldToScreenPoint(transform.position).z;

        cardHalo.SetActive(false);
    }

    public virtual void OnMouseDown()
    {
        basePosition = transform.position;
        deckManager.dragPlane.SetActive(true);
        transform.localRotation = Quaternion.Euler(90, 0, 0);
    }


    public abstract void OnMouseDrag();

    public abstract void OnMouseUp();

    public void OnMouseOver()
    {
        Debug.Log("Over the Card");

        if (Input.GetMouseButtonDown(1)) //Récupère la carte sur laquelle le joueur clique
        {

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (player.statePlayer != StatePlayer.ZoomPhase)
            {
                Debug.Log("OUVRE FDP");
                if (Physics.Raycast(ray, out hit, 100, cardMask))
                {
                    Debug.Log(hit.transform.position);
                    zoomedCard = hit.transform.gameObject;
                    positionBeforeZoom = hit.transform.position;
                    rotationBeforeZoom = hit.transform.rotation;
                    Debug.Log(positionBeforeZoom);

                    hit.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - 6f, Camera.main.transform.position.z + 4f);

                    hit.transform.rotation = Camera.main.transform.rotation;
                    phaseBeforeZoom = player.statePlayer;
                    player.statePlayer = StatePlayer.ZoomPhase;

                }
            }
            else if (player.statePlayer == StatePlayer.ZoomPhase && zoomedCard != null)
            {
                Debug.Log(positionBeforeZoom);
                Debug.Log("FERME BATARD");
                zoomedCard.transform.position = positionBeforeZoom;
                zoomedCard.transform.rotation = rotationBeforeZoom;
                player.statePlayer = phaseBeforeZoom;
                cardIsZoom = false;
            }
        }
    }


    protected Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToViewportPoint(mousePoint);
    }
    
}
