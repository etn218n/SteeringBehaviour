using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomPoint : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private float duration;
    
    private void Start()
    {
        StartCoroutine(RandomizePosition(duration));
    }

    private IEnumerator RandomizePosition(float duration)
    {
        transform.position = Random.insideUnitSphere * radius;

        yield return new WaitForSeconds(duration);
        
        StartCoroutine(RandomizePosition(duration));
    }
}
