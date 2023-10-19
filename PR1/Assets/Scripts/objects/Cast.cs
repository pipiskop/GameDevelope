using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float speed = 5f; // Скорость движения фаербола

    void Update()
    {
        // Двигаем фаербол вперед
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Обработка столкновения с другими объектами
        if (other.CompareTag("Enemy"))
        {
            // Реакция на столкновение с врагом, например, уничтожение врага
            Destroy(other.gameObject);

            // Уничтожаем фаербол
            Destroy(gameObject);
        }
    }
}
