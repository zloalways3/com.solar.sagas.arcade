using SolarSaga.SoundsManagerSolar;
using UnityEngine;

public class Plane : MonoBehaviour
{
    [SerializeField] AudioSource PlaneSound;   
    
    [SerializeField] GameManager gameManager;
    private float positionPlane = 0;
    public float PosPlane { get { return positionPlane; } set { positionPlane = value;
            positionPlane = Mathf.Min(positionPlane, 2.51f);
            positionPlane = Mathf.Max(positionPlane, -2.51f);
        } }

    void Update()
    {
        if(gameManager.invali) transform.position = new Vector3 (positionPlane, transform.position.y, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag!="Bomb")
        {
            PlaneSound.PlayOneShottSoundManaged(PlaneSound.clip);
            gameManager.countScore++;
            if (gameManager.countScore >= (gameManager.LevelConst+1)*50) gameManager.EndGame();
            Destroy(collision.gameObject);  
        } else
        {
            gameManager.EndGame();
        }
    }
}
