using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if(ScoreManager.instance != null)
                ScoreManager.instance.AddScore(coinValue);

            Destroy(gameObject);
        }
    }
}
