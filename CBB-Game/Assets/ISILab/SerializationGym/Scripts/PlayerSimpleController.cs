using UnityEngine;

public class PlayerSimpleController : MonoBehaviour
{
    // Simple controller to the player that uses the old input system of Unity
    // It handles movement horizontally and vertically
    [SerializeField]
    private float speed = 5f;
    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        transform.position += speed * Time.deltaTime * new Vector3(horizontal, 0, vertical);
    }

}
