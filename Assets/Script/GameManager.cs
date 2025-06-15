using UnityEngine;


public class GameManager : MonoBehaviour

{
    public static GameManager instance;
    public float worldSpeed;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;  // set instance pertama kali
        }
        else
        {
            Destroy(gameObject);  // hancurkan duplikat
        }
    }




}
