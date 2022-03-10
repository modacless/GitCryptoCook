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
    public float hoverScaleMultiplier;
    

    #region r�f�rences
    public DeckManager deckManager;

    public GameObject cardHalo;
    #endregion

    #region Drag parameters
    protected float mZCoord;
    protected Vector3 basePosition;
    protected Quaternion baseRotation;
    #endregion

    public PlayerBehavior player;

    private UnityEvent OnUse;

    

    void Start()
    {
        deckManager = GameObject.Find("GameManager").GetComponent<DeckManager>();
        mZCoord = Camera.main.WorldToScreenPoint(transform.position).z;

        cardHalo.SetActive(false);
        baseScale = transform.localScale;
        targetScale = baseScale;
    }

    public virtual void OnMouseDown()
    {
        if(!deckManager.authorityPlayer.cardIsZoom)
        {
            SetCurrentPosAsBase();
            deckManager.dragPlane.SetActive(true);
            transform.localRotation = Quaternion.Euler(90, 180, 0);

            AudioManager.AMInstance.PlaySFX(AudioManager.AMInstance.PickCardSFX, 2f);

            if (isServer)
            {
                transform.localRotation = Quaternion.Euler(90, 0, 0);
            }
        }
       
        
    }


    public abstract void OnMouseDrag();

    public abstract void OnMouseUp();

    private void Update()
    {
        UpdateHover();
    }

    public virtual void OnMouseExit()
    {
        targetScale = baseScale;
    }

    public virtual void OnMouseEnter()
    {
        AudioManager.AMInstance.PlaySFX(AudioManager.AMInstance.mouseOverCardSFX, 0.1f);
        targetScale = baseScale * hoverScaleMultiplier;
    }

    protected Vector3 baseScale;
    protected Vector3 targetScale;
    public void UpdateHover()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, 30 * Time.deltaTime);
    }

    public void ResetPos()
    {
        transform.position = basePosition;
        transform.rotation = baseRotation;
    }

    public void SetCurrentPosAsBase()
    {
        basePosition = transform.position;
        baseRotation = transform.rotation;
    }

    protected Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToViewportPoint(mousePoint);
    }
    
}
