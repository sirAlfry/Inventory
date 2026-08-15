using UnityEngine;
using InventorySystem.Items;
using InventorySystem.Core;

namespace InventorySystem.World
{
    /// <summary>
    /// Componente da mettere su un prefab di pickup nel mondo di gioco. Quando il collider
    /// trigger del player entra nel trigger di questo pickup, l'item viene aggiunto
    /// all'Inventory del player e il pickup si rimuove da solo.
    /// Richiede un Collider con "Is Trigger" attivo su questo GameObject.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ItemPickup : MonoBehaviour
    {
        [SerializeField] private ItemData item;
        [SerializeField] private int quantity = 1;
        [SerializeField] private string playerTag = "Player";

        [Header("Feedback (opzionale)")]
        [SerializeField] private GameObject pickupVfx;
        [SerializeField] private AudioClip pickupSfx;

        private void Reset()
        {
            // Comodita' per non dimenticarsi di impostare il collider correttamente.
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(playerTag)) return;
            if (item == null) return;

            // GetComponentInParent perche' spesso il collider del player e' su un figlio
            // (es. una CapsuleCollider su un child "Hitbox") mentre Inventory sta sul root.
            var inventory = other.GetComponentInParent<Inventory>();
            if (inventory == null)
            {
                Debug.LogWarning($"[ItemPickup] '{other.name}' non ha un componente Inventory.");
                return;
            }

            int leftover = inventory.AddItem(item, quantity);
            int picked = quantity - leftover;

            if (picked <= 0) return; // inventario pieno, il pickup resta nel mondo cosi' com'e'

            if (pickupVfx != null) Instantiate(pickupVfx, transform.position, Quaternion.identity);
            if (pickupSfx != null) AudioSource.PlayClipAtPoint(pickupSfx, transform.position);

            if (leftover > 0)
            {
                // Inventario pieno a meta' raccolta: il pickup resta nel mondo con la
                // sola quantita' rimasta, cosi' il giocatore puo' tornare a raccoglierla.
                quantity = leftover;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
