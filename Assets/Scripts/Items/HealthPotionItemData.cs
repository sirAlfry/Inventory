using UnityEngine;
using InventorySystem.Player;

namespace InventorySystem.Items
{
    /// <summary>
    /// Item consumabile che ripristina salute a chiunque lo utilizzi.
    /// [CreateAssetMenu] permette ai designer di creare nuove varianti di bilanciamento
    /// (es. "Pozione Piccola" cura=20, "Pozione Grande" cura=75) da menu contestuale,
    /// senza scrivere codice: Assets -> Create -> Inventory -> Items -> Health Potion.
    /// </summary>
    [CreateAssetMenu(menuName = "Inventory/Items/Health Potion", fileName = "New Health Potion")]
    public class HealthPotionItemData : ItemData
    {
        [Header("Impostazioni Pozione")]
        [SerializeField] private float healAmount = 25f;

        public override bool Use(GameObject user)
        {
            // Disaccoppiato tramite IHealth: funziona con qualsiasi componente che la
            // implementi, non solo con una classe PlayerHealth cablata staticamente.
            if (user.TryGetComponent<IHealth>(out var health))
            {
                health.Heal(healAmount);
                return true; // consumato dopo l'uso
            }

            Debug.LogWarning($"[HealthPotionItemData] '{user.name}' non ha un componente IHealth.");
            return false; // niente e' successo, l'item resta nell'inventario
        }
    }
}
