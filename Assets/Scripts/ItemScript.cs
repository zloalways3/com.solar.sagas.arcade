using UnityEngine;

public class ItemScript : MonoBehaviour
{

    Rigidbody2D ItemBody;
    [SerializeField] Sprite[] ItemSkinsSprites;
    void Start()
    {
        ItemBody = GetComponent<Rigidbody2D>();
        SpriteRenderer spriteItem = GetComponent<SpriteRenderer>();
        if(gameObject.tag!="Bomb") spriteItem.sprite = ItemSkinsSprites[Random.Range(0, ItemSkinsSprites.Length)];
    }

    void Update()
    {
        ItemBody.velocity = new Vector2(0,-2f) + Vector2.zero;
        if(transform.position.y<-10f)
        {
            Destroy(gameObject);
        }
    }
}
