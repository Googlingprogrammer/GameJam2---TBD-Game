using UnityEngine;

public class CollisionTrigger : MonoBehaviour
{
    public GameObject objectToEnable;
    public AudioSource DoorSFX;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the object you want to trigger the activation
        if (other.gameObject.CompareTag("Player")) // Change "Player" to the tag of the object you want to collide with
        {
            // Enable the specified GameObject
            if (objectToEnable != null)
            {
                objectToEnable.SetActive(true);
                DoorSFX.Play();
            }
        }
    }
}
