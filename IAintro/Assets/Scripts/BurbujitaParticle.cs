using UnityEngine;

public class BurbujitaParticle : MonoBehaviour
{
    [Header("Bubbles Settings")]
    [SerializeField] ParticleSystem burbujitas;

    [SerializeField] private float emissionMultiplier = 5f; // Cuántas burbujas por unidad de velocidad
    [SerializeField] private float minSpeedToEmit = 0.15f;    // Umbral para empezar a emitir
    [SerializeField] private float max = 50f;

    private SteeringBehaviour steering;
    private ParticleSystem.EmissionModule emission;
    private ParticleSystem.MainModule main;

    void Start()
    {
        steering = GetComponent<SteeringBehaviour>();

        if (burbujitas == null)
        {
            burbujitas = GetComponentInChildren<ParticleSystem>();
        } else if (burbujitas != null)
          {
            emission = burbujitas.emission;
            main = burbujitas.main;
          }
    }

    void Update()
    {
        if (burbujitas == null || steering == null) return;

        // Calcular velocidad actual
        float currentSpeed = steering.MaxVelocity * (steering.transform.InverseTransformDirection(steering.transform.position).magnitude);

        // Si el pez no se mueve, apagamos o reducimos la emisión
        float rate = Mathf.Clamp(currentSpeed * emissionMultiplier, 0, max);
        emission.rateOverTime = (currentSpeed > minSpeedToEmit) ? rate : 0f;

        // Ajustar tamaño y color según velocidad. Estos valores son los que he considerado mientras iba haciendo pruebas, pero deberían serializarse.
        main.startSize = Mathf.Lerp(0.05f, 0.15f, currentSpeed / steering.MaxVelocity);
        main.startColor = new Color(1f, 1f, 1f, Mathf.Lerp(0.2f, 0.6f, currentSpeed / steering.MaxVelocity));
    }   
}
