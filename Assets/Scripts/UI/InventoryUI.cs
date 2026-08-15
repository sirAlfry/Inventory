using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InventorySystem.Core;

namespace InventorySystem.UI
{
    /// <summary>
    /// Costruisce e aggiorna la griglia di InventorySlotUI a partire dai dati di un
    /// Inventory, e possiede l'icona "fluttuante" che segue il cursore mentre il
    /// giocatore trascina un item da uno slot all'altro.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private Inventory inventory;
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private Transform slotContainer;   // es. un oggetto con GridLayoutGroup
        [SerializeField] private GameObject itemUser;        // GameObject su cui vengono usati gli item (di solito il Player)

        [Header("Drag Visuals")]
        [SerializeField] private Canvas dragLayerCanvas;     // canvas di primo piano, disegna l'icona sopra tutti gli slot
        [SerializeField] private Image dragIconPrefab;

        private readonly List<InventorySlotUI> slotUIs = new();
        private Image activeDragIcon;

        public GameObject ItemUser => itemUser;

        private void Start()
        {
            BuildSlots();
            inventory.OnInventoryChanged += RefreshAll;
            RefreshAll();
        }

        private void OnDestroy()
        {
            // Fondamentale: senza questa unsubscribe, distruggere la UI mentre Inventory
            // resta vivo lascerebbe un riferimento "fantasma" nell'evento -> memory leak
            // e NullReferenceException al prossimo AddItem/UseItem.
            if (inventory != null)
                inventory.OnInventoryChanged -= RefreshAll;
        }

        private void BuildSlots()
        {
            for (int i = 0; i < inventory.SlotCount; i++)
            {
                var go = Instantiate(slotPrefab, slotContainer);
                var slotUI = go.GetComponent<InventorySlotUI>();
                slotUI.Initialize(inventory, this, i);
                slotUIs.Add(slotUI);
            }
        }

        private void RefreshAll()
        {
            for (int i = 0; i < slotUIs.Count; i++)
                slotUIs[i].Refresh(inventory.Slots[i]);
        }

        // ---- Gestione dell'icona di drag, centralizzata qui cosi' InventorySlotUI non deve
        // ---- conoscere il Canvas di primo piano ne' gestire la propria istanza fluttuante.

        public void BeginDrag(Sprite icon, Color tint, Vector2 screenPosition)
        {
            activeDragIcon = Instantiate(dragIconPrefab, dragLayerCanvas.transform);
            activeDragIcon.sprite = icon;
            activeDragIcon.color = tint; // mantiene lo stesso colore dello slot durante il trascinamento
            activeDragIcon.raycastTarget = false; // non deve mai bloccare i raycast, altrimenti OnDrop colpirebbe l'icona stessa
            activeDragIcon.rectTransform.position = screenPosition;
        }

        public void UpdateDrag(Vector2 screenPosition)
        {
            if (activeDragIcon != null)
                activeDragIcon.rectTransform.position = screenPosition;
        }

        public void EndDrag()
        {
            if (activeDragIcon != null)
                Destroy(activeDragIcon.gameObject);
        }
    }
}
