using UnityEngine;

public class SplashFX : MonoBehaviour
{
    private float lifeTime = 1f;
    private float elapsed = 0f;
    private SpriteRenderer sr;
    private Vector3 startScale;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        startScale = transform.localScale;
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        float t = elapsed / lifeTime;

        // Escalado de 1x a 3x
        transform.localScale = startScale * Mathf.Lerp(1f, 3f, t);

        // Desvanecimiento alfa
        Color c = sr.color;
        c.a = Mathf.Lerp(0.3f, 0f, t);
        sr.color = c;

        if (elapsed >= lifeTime)
            Destroy(gameObject);
    }
}