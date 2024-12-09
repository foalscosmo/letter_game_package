using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class LetterDrag : MonoBehaviour
{
    private Vector3 originalPosition;
    private Vector3 offset;
    private bool isDragging = false;
    private bool isPlaced = false;
    private bool isSnapping = false;

    public List<Slot> validSlots;
    public List<string> correctTags;
    public List<string> wrongTags;

    private LetterSpawner _Spawner;

    public float snapThreshold = 1.5f;
    public float snapSpeed = 3f;
    public GameObject BackgroundFrame;
    private SpriteRenderer _SpriteRenderer;

    public float shakeAmount = 0.2f;
    public float shakeDuration = 0.5f;

    public Animator animator;
    public ParticleSystem particle;

    /*private void Awake()
    {
        particle.Stop(particle);
    }*/

    private void Start()
    {
        _Spawner = FindObjectOfType<LetterSpawner>();
        _SpriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isPlaced || isSnapping) return;

        if (Input.touchCount > 0)
        {
            HandleTouchInput();
        }
        else
        {
            HandleMouseInput();
        }
    }

    private void HandleTouchInput()
    {
        Touch touch = Input.GetTouch(0);
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));

        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (IsTouchingObject(touchPosition))
                {
                    StartDragging(touchPosition);
                }
                break;

            case TouchPhase.Moved:
                if (isDragging)
                {
                    DragObject(touchPosition);
                }
                break;

            case TouchPhase.Ended:
                if (isDragging)
                {
                    StopDragging();
                }
                break;
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            if (IsTouchingObject(mousePosition))
            {
                StartDragging(mousePosition);
            }
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            DragObject(mousePosition);
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            StopDragging();
        }
    }
    private bool IsTouchingObject(Vector3 position)
    {
        Collider2D hitCollider = Physics2D.OverlapPoint(position);
        return hitCollider != null && hitCollider.gameObject == gameObject;
    }

    private void DragObject(Vector3 position)
    {
        transform.position = new Vector3(position.x + offset.x, position.y + offset.y, 0);
        _SpriteRenderer.sortingOrder = 5;
    }

    private void StopDragging()
    {
        isDragging = false;
        CheckPlacement();
    }
    private Slot GetTargetSlot(Transform closestTarget)
    {
        foreach (Slot slot in FindObjectsOfType<Slot>())
        {
            Debug.Log(0);

            if (Vector3.Distance(slot.transform.position, closestTarget.position) < 0.1f)
            {
                Debug.Log(1);

                return slot;
            }
        }
        return null;
    }
    private void CheckPlacement()
    {
        Transform closestTarget = GetClosestTarget();
        if (closestTarget != null)
        {
            Slot targetSlot = GetTargetSlot(closestTarget);
            if (targetSlot != null && !targetSlot.IsOccupied)
            {
                StartCoroutine(SmoothSnapToTargetPosition(closestTarget.position, targetSlot));
            }
            else
            {
                StartCoroutine(SmoothSnapToTargetPosition(originalPosition, null));
            }
        }
        else
        {
            StartCoroutine(SmoothSnapToTargetPosition(originalPosition, null));
        }
    }

    private Transform GetClosestTarget()
    {
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var slot in validSlots)
        {
            if (slot == null || slot.IsOccupied || !correctTags.Contains(slot.tag)) continue;

            float distance = Vector3.Distance(transform.position, slot.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = slot.transform;
            }
        }

        return closestDistance < snapThreshold ? closestTarget : null;
    }


    private IEnumerator SmoothSnapToTargetPosition(Vector3 targetPosition, Slot targetSlot)
    {
        isSnapping = true;
        float time = 0f;
        Vector3 startPosition = transform.position;

        while (time < 1f)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time);
            time += Time.deltaTime * snapSpeed;
            yield return null;
        }

        transform.position = targetPosition;
        isSnapping = false;

        if (targetPosition != originalPosition)
        {
            isPlaced = true;
            animator.SetTrigger("Snap");
            yield return new WaitForSeconds(0.7f);
            BackgroundFrame?.SetActive(true);
            particle.Play();
            targetSlot.IsOccupied = true;
            _Spawner?.ObjectPlaced(targetSlot);
        }
    }


    private void StartDragging(Vector3 position)
    {
        if (isSnapping) return;

        originalPosition = transform.position;
        offset = transform.position - position;
        isDragging = true;
    }

    public void StartSnapAnimation()
    {
        animator.SetTrigger("Snap");
    }
}
