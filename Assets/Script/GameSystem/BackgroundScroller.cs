using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.1f;

    private Material material;
    private Vector2 offset;

    void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
        offset = material.mainTextureOffset;
    }

    void Update()
    {
        offset.x += scrollSpeed * Time.deltaTime;
        material.mainTextureOffset = offset;
    }
}