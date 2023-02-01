using System;
using Cards;
using Managers;
using Rooms;
using UnityEngine;

namespace Interactables
{
  public abstract class Interactable : MonoBehaviour
  {
    #region Serialized Fields

    [SerializeField] private Sprite _beforeInteractSprite;
    [SerializeField] private Sprite _afterInteractSprite;
    
    [SerializeField] private Sprite _beforeInteractInnerSprite;
    [SerializeField] private Sprite _afterInteractInnerSprite;
    
    [SerializeField] private SpriteRenderer _innerRenderer;
    
    [SerializeField] private Material _outlineMaterial;

    [SerializeField] private GameObject _interactText;

    #endregion

    #region Non-Serialized Fields

    private SpriteRenderer _renderer;
    private Material _material;
    private Material _defaultMaterial;
    private bool _after = false;
    private Action _interactAction;
    private Animator _animator;
    private static readonly int Interact = Animator.StringToHash("Interact");

    #endregion

    #region Properties
    
    public RoomNode ParentNode { get; set; }

    #endregion

    #region Function Events

    private void Awake()
    {
      _renderer = GetComponent<SpriteRenderer>();
      _animator = GetComponentInChildren<Animator>();

      _renderer.sprite = _beforeInteractSprite;
      _innerRenderer.sprite = _beforeInteractInnerSprite;

      _defaultMaterial = _renderer.material;
      _material = _outlineMaterial == null ? _defaultMaterial : new Material(_outlineMaterial);

      _interactAction = () =>
      {
        ParentNode.Interacted = true;
        SetToAfter();
        OnInteract();
      };
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      if(_after) return;
      
      if (other.CompareTag("Player"))
      {
        _innerRenderer.material = _material;
        _interactText.SetActive(true);
        GameManager.PlayerController.InteractEvent = _interactAction;
      }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
      if(_after) return;
      
      if (other.CompareTag("Player"))
      {
        _innerRenderer.material = _defaultMaterial;
        _interactText.SetActive(false);
        GameManager.PlayerController.InteractEvent = null;
      }
    }

    #endregion

    #region Public Methods

    public void SetToAfter()
    {
      if (_animator != null)
      {
        _animator.SetTrigger(Interact);
      }
      _renderer.sprite = _afterInteractSprite;
      _innerRenderer.sprite = _afterInteractInnerSprite;
      _innerRenderer.material = _defaultMaterial;
      _interactText.SetActive(false);
      _after = true;
    }

    #endregion

    #region Private Methods

    protected abstract void OnInteract();

    #endregion
  }
}

