using UnityEngine;

public class BlockController : MonoBehaviour
{
    [HideInInspector]
    public TetrisController parentController;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("êGÇ¡ÇΩ: " + collision.gameObject.name);

        // Block É^ÉOÇÃÇ‡ÇÃÇ…êGÇÍÇΩÇ∆Ç´ÇæÇØí‚é~
        if (collision.gameObject.CompareTag("Block"))
        {
            parentController.StopCurrentBlock();
        }
    }
}
