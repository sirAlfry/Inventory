using System;
using InventorySystem.Items;

namespace InventorySystem.Core
{
    /// <summary>
    /// Classe dati serializzabile che rappresenta uno slot dell'inventario.
    /// Tenuta separata da MonoBehaviour: e' leggera, facile da testare isolatamente
    /// e facile da serializzare (es. con JsonUtility) per un futuro sistema di salvataggio.
    /// </summary>
    [Serializable]
    public class InventorySlot
    {
        public ItemData Item { get; private set; }
        public int Quantity { get; private set; }

        public bool IsEmpty => Item == null || Quantity <= 0;

        public InventorySlot() => Clear();

        /// <summary>
        /// Tenta di aggiungere quantita' a questo slot. Ritorna la quantita' che NON
        /// e' entrata (0 = tutto aggiunto), cosi' il chiamante puo' riversarla nello slot
        /// successivo (utile quando uno stack raggiunge la dimensione massima).
        /// </summary>
        public int Add(ItemData item, int amount)
        {
            if (IsEmpty)
            {
                Item = item;
                Quantity = 0;
            }
            else if (Item != item)
            {
                return amount; // slot occupato da un item diverso, niente entra
            }

            int spaceLeft = Item.MaxStackSize - Quantity;
            int amountToAdd = Math.Min(spaceLeft, amount);
            Quantity += amountToAdd;
            return amount - amountToAdd;
        }

        public void Remove(int amount)
        {
            Quantity = Math.Max(0, Quantity - amount);
            if (Quantity == 0) Clear();
        }

        public void Clear()
        {
            Item = null;
            Quantity = 0;
        }

        /// <summary>Scambia il contenuto con un altro slot: e' l'operazione alla base del riordino via drag & drop.</summary>
        public void SwapWith(InventorySlot other)
        {
            (Item, other.Item) = (other.Item, Item);
            (Quantity, other.Quantity) = (other.Quantity, Quantity);
        }

        /// <summary>
        /// Sovrascrive direttamente il contenuto dello slot. Usato internamente da Inventory
        /// quando serve impostare uno slot in modo esplicito (es. dopo un merge di stack).
        /// </summary>
        public void Set(ItemData item, int quantity)
        {
            Item = item;
            Quantity = quantity;
            if (Quantity <= 0) Clear();
        }
    }
}
