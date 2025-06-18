using UnityEngine;

public class LaserWeapon : MonoBehaviour
{
    public static LaserWeapon Instance;

    [SerializeField] private GameObject prefab;

    public float speed;
    public float damage;

    void Awake()
    {
        if (Instance == null) Instance = this;

        else
            Destroy(gameObject);
    }

    public void Shoot()
    {
        Vector3 spawnPos = PlayerController.instance.transform.position + new Vector3(0.6f, 0f, 0f);
        GameObject bullet = Instantiate(prefab, spawnPos, Quaternion.identity);

        // Inisialisasi arah tembakan
        LaserBullets bulletScript = bullet.GetComponent<LaserBullets>();
        if (bulletScript != null)
        {
            bulletScript.Initialize(Vector2.right); // default ke kanan
        }
    }






    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
