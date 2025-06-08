using UnityEngine;

[System.Serializable]
public class TowerFirePointHelper : MonoBehaviour
{
    [Header("Configuração Rápida")]
    [SerializeField] private bool useAutoConfig = false;
    [SerializeField] private int numberOfCannons = 1;
    [SerializeField] private float cannonSpacing = 0.5f;
    [SerializeField] private Vector3 baseOffset = Vector3.up * 0.5f;
    
    [Header("Debug")]
    [SerializeField] private bool showFirePoints = true;
    [SerializeField] private Color gizmoColor = Color.red;
    [SerializeField] private float gizmoSize = 0.1f;

    private Tower towerScript;

    void Awake()
    {
        towerScript = GetComponent<Tower>();
        if (useAutoConfig)
        {
            AutoConfigureFirePoints();
        }
    }

    void AutoConfigureFirePoints()
    {
        if (towerScript == null) return;

        Vector3[] offsets = new Vector3[numberOfCannons];
        
        if (numberOfCannons == 1)
        {
            // Um canhão central
            offsets[0] = baseOffset;
        }
        else if (numberOfCannons == 2)
        {
            // Dois canhões lado a lado
            offsets[0] = baseOffset + Vector3.left * (cannonSpacing / 2f);
            offsets[1] = baseOffset + Vector3.right * (cannonSpacing / 2f);
        }
        else
        {
            // Múltiplos canhões distribuídos
            float startOffset = -(numberOfCannons - 1) * cannonSpacing / 2f;
            for (int i = 0; i < numberOfCannons; i++)
            {
                offsets[i] = baseOffset + Vector3.right * (startOffset + i * cannonSpacing);
            }
        }

        towerScript.fireOffsets = offsets;
        Debug.Log($"TowerFirePointHelper: Configurados {numberOfCannons} pontos de tiro para {gameObject.name}");
    }

    void OnDrawGizmosSelected()
    {
        if (!showFirePoints) return;

        Gizmos.color = gizmoColor;
        
        // Desenhar pontos de Transform se existirem
        if (towerScript != null && towerScript.firePoints != null)
        {
            foreach (Transform firePoint in towerScript.firePoints)
            {
                if (firePoint != null)
                {
                    Gizmos.DrawWireSphere(firePoint.position, gizmoSize);
                    Gizmos.DrawLine(transform.position, firePoint.position);
                }
            }
        }
        
        // Desenhar offsets se existirem
        if (towerScript != null && towerScript.fireOffsets != null)
        {
            foreach (Vector3 offset in towerScript.fireOffsets)
            {
                Vector3 worldPos = transform.position + offset;
                Gizmos.DrawWireSphere(worldPos, gizmoSize);
                Gizmos.DrawLine(transform.position, worldPos);
            }
        }
    }

    // Método para reconfigurar em runtime
    [ContextMenu("Reconfigurar Pontos de Tiro")]
    public void ReconfigureFirePoints()
    {
        AutoConfigureFirePoints();
    }
} 