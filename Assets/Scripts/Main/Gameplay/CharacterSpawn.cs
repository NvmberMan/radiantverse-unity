using UnityEngine;

public class CharacterSpawn : MonoBehaviour
{
    public float fallThreshold = -15f;
    public Vector3 spawnPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPoint = transform.position;
    }


    private void Update()
    {
        if (transform.position.y < fallThreshold)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        transform.position = spawnPoint;
    }

}
