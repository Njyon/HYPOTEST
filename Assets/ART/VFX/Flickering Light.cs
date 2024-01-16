using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    public float flickerSpeed = 5.0f;
    public float minIntensity = 0.2f;
    public float maxIntensity = 1.0f;
    public float intensityChangeInterval = 0.1f;

    private float targetIntensity;

    private void Start()
    {
        targetIntensity = Random.Range(minIntensity, maxIntensity);
    }

    private void Update()
    {
        GetComponent<Light>().intensity = Mathf.MoveTowards(GetComponent<Light>().intensity, targetIntensity, Time.deltaTime * flickerSpeed);

        if (GetComponent<Light>().intensity == targetIntensity)
        {
            targetIntensity = Random.Range(minIntensity, maxIntensity);
            Invoke("ChangeIntensity", intensityChangeInterval);
        }

        if (GetComponent<Light>().intensity < minIntensity)
        {
            GetComponent<Light>().intensity = minIntensity;
        }
    }

    private void ChangeIntensity()
    {
        targetIntensity = Random.Range(minIntensity, maxIntensity);
    }
}

