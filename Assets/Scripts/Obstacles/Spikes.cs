using UnityEngine;

public class Spikes : MonoBehaviour
{
    public int Damage = 50;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Small"))
        {
            var smallHp = collision.GetComponent<SmallHpScripts>();
            if (smallHp != null)
            {
                smallHp.TakeDamage(Damage); 
            }
        }
        //else if (collision.CompareTag("Big"))
        //{
        //    var bigHp = collision.GetComponent<BigHpScripts>();
        //    if (bigHp != null)
        //    {
        //        bigHp.TakeDamage(Damage , false);
        //    }
        //}
    }
}
