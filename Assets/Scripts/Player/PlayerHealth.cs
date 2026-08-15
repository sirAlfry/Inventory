using UnityEngine;

namespace InventorySystem.Player
{
    /// <summary>
    /// Implementazione dimostrativa minimale di IHealth, cosi' HealthPotionItemData ha
    /// subito qualcosa con cui interagire "out of the box". Nel vostro progetto reale
    /// sostituite/estendete questo script con il vostro sistema di salute: finche'
    /// implementa IHealth, la pozione continuera' a funzionare senza modifiche.
    /// </summary>
    public class PlayerHealth : MonoBehaviour, IHealth
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;

        private void Awake() => currentHealth = maxHealth;

        public void Heal(float amount)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            Debug.Log($"[PlayerHealth] Curato di {amount} HP. Attuale: {currentHealth}/{maxHealth}");
        }

        public void Damage(float amount)
        {
            currentHealth = Mathf.Max(0f, currentHealth - amount);
            Debug.Log($"[PlayerHealth] Subito {amount} danno. Attuale: {currentHealth}/{maxHealth}");
        }
    }
}
