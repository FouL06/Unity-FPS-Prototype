using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{

    public int startingHealth = 100;

    [SyncVar]
    public int currentHealth;
    public Text healthText;
    public Slider healthSlider;
    private FpsController fpsController;

    void Start()
    {
        currentHealth = startingHealth;
        fpsController = GetComponent<FpsController>();
    }

    void Update()
    {
        if (currentHealth > 0)
        {
            if (healthSlider.value - currentHealth >= 5)
            {
                healthSlider.value -= 5;
            }
        }
        else
        {
            healthSlider.value = 0;
            currentHealth = 0;
            Die();
        }
        UpdateHealth();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
    }

    void UpdateHealth()
    {
        healthSlider.value = currentHealth;
        healthText.text = currentHealth.ToString();
    }

    void Die()
    {
        currentHealth = startingHealth;
        fpsController.ResetPlayer();
        // TODO: Call respawn method here
        // TODO: Call method in scoreManager to adjust new score from this player dieing
    }
}
