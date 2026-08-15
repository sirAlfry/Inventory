using UnityEngine;

namespace InventorySystem.Items
{
    /// <summary>
    /// Classe base astratta per ogni oggetto di gioco, implementata come ScriptableObject
    /// cosi' i designer possono creare nuovi item come asset nel Project window senza
    /// toccare il codice (Create -> Inventory -> Items -> ...).
    ///
    /// Scelta progettuale: invece di una singola classe "Item" con uno switch su un enum
    /// per decidere il comportamento, ogni tipo concreto di item (vedi HealthPotionItemData,
    /// SpeedBoostItemData) ridefinisce Use() con il proprio effetto. Questo segue l'Open/Closed
    /// Principle: aggiungere un nuovo tipo di item richiede solo una nuova sottoclasse,
    /// senza modificare Inventory o l'interfaccia utente.
    /// </summary>
    public abstract class ItemData : ScriptableObject
    {
        [Header("Identita'")]
        [SerializeField] private string itemId = System.Guid.NewGuid().ToString();
        [SerializeField] private string itemName = "New Item";
        [TextArea] [SerializeField] private string description;
        [SerializeField] private Sprite icon;

        [Header("Aspetto rapido senza texture esterne")]
        [Tooltip("Colore applicato all'icona in UI. Utile per distinguere gli item usando " +
                 "un unico sprite bianco built-in di Unity (es. Assets > Create > 2D > Sprites > Square) " +
                 "invece di dover creare texture in un editor di immagini esterno.")]
        [SerializeField] private Color iconTint = Color.white;

        [Header("Stacking")]
        [SerializeField] private bool isStackable = true;
        [SerializeField] private int maxStackSize = 99;

        public string ItemId => itemId;
        public string ItemName => itemName;
        public string Description => description;
        public Sprite Icon => icon;
        public Color IconTint => iconTint;
        public bool IsStackable => isStackable;

        // Un item non stackabile ha sempre dimensione massima dello stack pari a 1,
        // cosi' non serve ricordarsi di tenerli sincronizzati manualmente nell'Inspector.
        public int MaxStackSize => isStackable ? maxStackSize : 1;

        /// <summary>
        /// Esegue l'effetto unico di questo item sull'utilizzatore.
        /// Ritorna true se l'item deve essere consumato (rimosso dall'inventario) dopo l'uso,
        /// false se e' un oggetto riutilizzabile/equipaggiabile che resta nell'inventario.
        /// </summary>
        public abstract bool Use(GameObject user);
    }
}
