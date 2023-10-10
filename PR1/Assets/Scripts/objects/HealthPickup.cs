using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float healthAmount = 20f; // Количество восстанавливаемых очков жизней

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Проверка, что объект, с которым произошло соприкосновение, является игроком
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Восстановление очков жизней игрока
                player.RestoreHealth(healthAmount);

                // Уничтожение объекта, так как он был подобран
                Destroy(gameObject);
            }
        }
    }
}
