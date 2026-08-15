using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using InventorySystem.Core;

namespace InventorySystem.UI
{
    /// <summary>
    /// Rappresentazione visiva di un singolo slot dell'inventario. Gestisce:
    ///  - la visualizzazione di icona + quantita';
    ///  - il click sinistro per usare l'item;
    ///  - il drag & drop per riorganizzare l'inventario, tramite le interfacce
    ///    standard dell'EventSystem di Unity (IBeginDragHandler, IDragHandler, ecc.).
    ///
    /// Nota implementativa sul drag & drop: non spostiamo l'elemento UI reale che viene
    /// trascinato. Invece cloniamo la sua icona su un Canvas "drag layer" di primo piano,
    /// cosi' si vede sopra tutti gli altri slot durante il trascinamento, e la seguiamo
    /// col puntatore. Al rilascio, lo slot su cui si e' rilasciato riceve OnDrop e chiede
    /// a Inventory di scambiare i due slot.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class InventorySlotUI : MonoBehaviour,
        IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Text quantityText; // sostituire con TMP_Text se si usa TextMeshPro

        private Inventory inventory;
        private InventoryUI ownerUI;
        private int slotIndex;
        private CanvasGroup canvasGroup;

        private void Awake() => canvasGroup = GetComponent<CanvasGroup>();

        /// <summary>Chiamato da InventoryUI subito dopo l'istanziazione di questo prefab.</summary>
        public void Initialize(Inventory inventory, InventoryUI ownerUI, int slotIndex)
        {
            this.inventory = inventory;
            this.ownerUI = ownerUI;
            this.slotIndex = slotIndex;
        }

        /// <summary>Aggiorna icona/quantita' visualizzate in base allo stato attuale dello slot.</summary>
        public void Refresh(InventorySlot slot)
        {
            bool hasItem = !slot.IsEmpty;

            iconImage.enabled = hasItem;
            iconImage.sprite = hasItem ? slot.Item.Icon : null;
            // Applica il colore configurato sull'ItemData: permette di distinguere gli item
            // usando un unico sprite bianco built-in (es. "Square") invece di texture esterne.
            iconImage.color = hasItem ? slot.Item.IconTint : Color.white;

            quantityText.gameObject.SetActive(hasItem && slot.Item.IsStackable && slot.Quantity > 1);
            quantityText.text = hasItem ? slot.Quantity.ToString() : string.Empty;
        }

        /// <summary>Click sinistro su uno slot usa l'item che contiene.</summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            inventory.UseItem(slotIndex, ownerUI.ItemUser);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (inventory.Slots[slotIndex].IsEmpty) return;

            // Disattiva temporaneamente i raycast sullo slot sorgente, cosi' OnDrop
            // rileva correttamente lo slot SOTTO il puntatore invece di se stesso;
            // e delega a InventoryUI la creazione dell'icona fluttuante.
            canvasGroup.blocksRaycasts = false;
            ownerUI.BeginDrag(iconImage.sprite, iconImage.color, eventData.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            ownerUI.UpdateDrag(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = true;
            ownerUI.EndDrag();
        }

        /// <summary>
        /// Chiamato sullo slot su cui il puntatore viene rilasciato (l'EventSystem di Unity
        /// invia IDropHandler all'elemento UI che si trova sotto il puntatore al momento
        /// del rilascio). Chiediamo a Inventory di scambiare lo slot trascinato con questo.
        /// </summary>
        public void OnDrop(PointerEventData eventData)
        {
            var draggedSlot = eventData.pointerDrag != null
                ? eventData.pointerDrag.GetComponent<InventorySlotUI>()
                : null;

            if (draggedSlot != null && draggedSlot != this)
                inventory.SwapSlots(draggedSlot.slotIndex, slotIndex);
        }
    }
}
