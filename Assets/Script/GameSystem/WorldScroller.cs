using UnityEngine;

public class WorldScroller : MonoBehaviour
{
    public float scrollSpeed = 2f;

    void Update()
    {
        Vector3 move = Vector3.left * scrollSpeed * Time.deltaTime;

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("WorldObject"))
        {
            obj.transform.position += move;
        }
    }
}
