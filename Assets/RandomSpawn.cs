using UnityEngine;

public class RandomSpawn : MonoBehaviour
{
    [SerializeField] private float radius;

    private void Start()
    {
        transform.position = Random.insideUnitSphere * radius;
    }
}
