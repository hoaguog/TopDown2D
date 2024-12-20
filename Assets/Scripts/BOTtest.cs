using UnityEngine;

public class BOTtest : MonoBehaviour
{
    public int heath = 100;
    public bool isDead = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"Heath = {heath}");

    }
    public void TakeDamage(int damage, string shooter)
    {
        heath -= damage;
        if (heath <= 0)
        {
            isDead = true;
            Destroy(gameObject);
        }
    }
}
