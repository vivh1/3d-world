using UnityEngine;

public class MiniMapScript : MonoBehaviour
{
    public Transform player;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void LateUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }
}
