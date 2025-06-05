using UnityEngine;
using UnityEngine.EventSystems;

public class TowerDrag : MonoBehaviour
{
    private Vector3 offset;
    private Camera mainCamera;
    private Vector3 originalPosition;
    private bool isDragging = false;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        if (Input.touchCount > 1) return; // ignora multitouch
        isDragging = true;
        originalPosition = transform.position;
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mousePos.x, mousePos.y, 0f);
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            Vector3 touchPos = mainCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
            transform.position = new Vector3(touchPos.x, touchPos.y, 0f) + offset;
        }
#else
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePos.x, mousePos.y, 0f) + offset;
#endif
    }

    void OnMouseUp()
    {
        isDragging = false;

        // verifica colisão com slots
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("TowerSlot"))
            {
                transform.position = hit.transform.position;
                return;
            }
        }

        // se não soltou em slot válido, volta pra posição original
        transform.position = originalPosition;
    }
}