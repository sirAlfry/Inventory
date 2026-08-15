using System;
using System.Collections.Generic;
using UnityEngine;
using InventorySystem.Items;

namespace InventorySystem.Core
{
    /// <summary>
    /// Contenitore centrale dell'inventario: una lista a dimensione fissa di InventorySlot,
    /// agganciata al player. Espone un evento cosi' un numero qualsiasi di viste UI (o altri
    /// sistemi, es. un quest tracker) possono reagire ai cambiamenti senza che Inventory
    /// debba conoscerli (pattern Observer). Questo mantiene la logica dati completamente
    /// separata dalla presentazione (vedi InventoryUI / InventorySlotUI).
    /// </summary>
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private int slotCount = 20;

        private List<InventorySlot> slots;

        /// <summary>
        /// Sollevato ogni volta che uno slot cambia, cosi' la UI puo' aggiornarsi.
        /// Nessun payload di proposito: mantiene l'evento semplice; chi ascolta rilegge
        /// l'intera lista di slot, operazione economica per la dimensione tipica di un inventario.
        /// </summary>
        public event Action OnInventoryChanged;

        public IReadOnlyList<InventorySlot> Slots => slots;
        public int SlotCount => slotCount;

        private void Awake()
        {
            slots = new List<InventorySlot>(slotCount);
            for (int i = 0; i < slotCount; i++)
                slots.Add(new InventorySlot());
        }

        /// <summary>
        /// Aggiunge uno stack di item all'inventario: riempie prima gli stack parziali
        /// esistenti dello stesso item, poi usa gli slot vuoti. Ritorna la quantita' che
        /// NON e' stato possibile aggiungere (0 = tutto aggiunto, inventario non pieno).
        /// </summary>
        public int AddItem(ItemData item, int quantity)
        {
            if (item == null || quantity <= 0) return quantity;

            int remaining = quantity;

            // 1) riempi prima gli stack esistenti dello stesso item
            if (item.IsStackable)
            {
                foreach (var slot in slots)
                {
                    if (remaining <= 0) break;
                    if (!slot.IsEmpty && slot.Item == item)
                        remaining = slot.Add(item, remaining);
                }
            }

            // 2) piazza il resto negli slot vuoti
            foreach (var slot in slots)
            {
                if (remaining <= 0) break;
                if (slot.IsEmpty)
                    remaining = slot.Add(item, remaining);
            }

            if (remaining != quantity)
                OnInventoryChanged?.Invoke();

            return remaining;
        }

        /// <summary>Rimuove fino a `quantity` unita' da uno slot specifico.</summary>
        public void RemoveFromSlot(int slotIndex, int quantity)
        {
            if (!IsValidIndex(slotIndex)) return;
            slots[slotIndex].Remove(quantity);
            OnInventoryChanged?.Invoke();
        }

        /// <summary>
        /// Usa l'item contenuto nello slot indicato su `user`. Se l'item segnala che deve
        /// essere consumato, un'unita' viene rimossa automaticamente dallo stack.
        /// </summary>
        public void UseItem(int slotIndex, GameObject user)
        {
            if (!IsValidIndex(slotIndex)) return;
            var slot = slots[slotIndex];
            if (slot.IsEmpty || user == null) return;

            bool consumed = slot.Item.Use(user);
            if (consumed)
                slot.Remove(1);

            OnInventoryChanged?.Invoke();
        }

        /// <summary>
        /// Scambia due slot. E' l'operazione che la UI di drag & drop invoca quando il
        /// giocatore rilascia un item su un altro slot, permettendo di riorganizzare
        /// liberamente la griglia dell'inventario.
        /// </summary>
        public void SwapSlots(int indexA, int indexB)
        {
            if (!IsValidIndex(indexA) || !IsValidIndex(indexB) || indexA == indexB) return;

            var a = slots[indexA];
            var b = slots[indexB];

            // Se entrambi gli slot contengono lo stesso item stackabile, uniamo gli stack
            // invece di scambiarli: trascinare una pozione su un'altra pila di pozioni le unisce.
            if (!a.IsEmpty && !b.IsEmpty && a.Item == b.Item && a.Item.IsStackable)
            {
                int leftover = b.Add(a.Item, a.Quantity);
                a.Set(a.Item, leftover); // se lo stack di destinazione era pieno, il resto rimane nello slot sorgente
            }
            else
            {
                a.SwapWith(b);
            }

            OnInventoryChanged?.Invoke();
        }

        private bool IsValidIndex(int index) => slots != null && index >= 0 && index < slots.Count;
    }
}
