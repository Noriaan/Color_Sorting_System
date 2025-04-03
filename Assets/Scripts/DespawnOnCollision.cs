using UnityEngine;

public class DespawnOnCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object we collided with has the "BOX" tag
        if (collision.gameObject.CompareTag("BOX"))
        {
            // Destroy this game object (the one this script is attached to)
            Destroy(gameObject);
        }
    }
}