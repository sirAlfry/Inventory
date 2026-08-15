using UnityEngine;
using InventorySystem.Player;

namespace InventorySystem.Items
{
    /// <summary>
    /// Item consumabile che concede un moltiplicatore temporaneo di velocita' di movimento.
    /// E' il secondo tipo di item richiesto dal task, e dimostra un effetto completamente
    /// diverso (buff nel tempo, invece di una cura istantanea) costruito sullo stesso
    /// contratto ItemData: nessuna modifica a Inventory, UI o pickup e' stata necessaria.
    /// </summary>
    [CreateAssetMenu(menuName = "Inventory/Items/Speed Boost", fileName = "New Speed Boost")]
    public class SpeedBoostItemData : ItemData
    {
        [Header("Impostazioni Speed Boost")]
        [SerializeField] private float speedMultiplier = 1.5f;
        [SerializeField] private float duration = 5f;

        public override bool Use(GameObject user)
        {
            if (user.TryGetComponent<ISpeedModifiable>(out var mover))
            {
                mover.ApplySpeedMultiplier(speedMultiplier, duration);
                return true;
            }

            Debug.LogWarning($"[SpeedBoostItemData] '{user.name}' non ha un componente ISpeedModifiable.");
            return false;
        }
    }
}
