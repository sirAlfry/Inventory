# Inventory System — Unity Programming Test

Prototipo Unity in prima persona realizzato per un "Programming Test for Unity Developer": un sistema di inventario completo e funzionante in cui il giocatore può raccogliere, visualizzare, usare e riorganizzare (drag & drop) gli oggetti in una scena 3D in prima persona.

## Funzionalità

- **Pickup → Inventario**: cammina sugli oggetti nel mondo per raccoglierli; le quantità stackabili vengono unite automaticamente.
- **Due tipi di item distinti**: una Pozione di Cura (cura all'uso) e uno Speed Boost (moltiplicatore di velocità temporaneo), ciascuno con il proprio effetto.
- **Click per usare**: click sinistro su uno slot per consumare/attivare l'item contenuto.
- **Drag & drop**: riorganizza la griglia dell'inventario trascinando gli slot uno sull'altro; trascinare su uno stack dello stesso item lo unisce.
- **Controller in prima persona**: movimento WASD/frecce relativo alla direzione dello sguardo, visuale col mouse (orizzontale + verticale) e salto.
- **Toggle del cursore**: `Esc` alterna tra visuale col mouse (cursore agganciato) e interazione con l'inventario (cursore libero).

## Stack tecnico

| | |
|---|---|
| Engine | Unity `6000.4.0f1` |
| Render pipeline | Universal Render Pipeline (URP) |
| Input | Input System package (nuovo), niente `UnityEngine.Input` legacy |
| Linguaggio | C# |
| UI | Unity UGUI (`Canvas` / `Text`) |

## Come iniziare

1. Clona il repository:
   ```bash
   git clone https://github.com/sirAlfry/Inventory.git
   ```
2. Apri il progetto con **Unity Hub**, versione `6000.4.0f1` (o una release compatibile di Unity 6).
3. Apri `Assets/Scenes/SampleScene.unity`.
4. Premi **Play**.

Non servono package esterni o asset a pagamento oltre a quelli già dichiarati in `Packages/manifest.json`, che include Input System e URP.

## Comandi

| Input | Azione |
|---|---|
| `W` `A` `S` `D` / Frecce | Movimento (relativo alla direzione dello sguardo) |
| Mouse | Guarda intorno (orizzontale + verticale) |
| `Spazio` | Salto (solo da terra) |
| Click sinistro su uno slot | Usa l'item in quello slot |
| Trascina uno slot | Sposta/scambia/unisce gli item nell'inventario |
| `Esc` | Blocca/sblocca il cursore (necessario per interagire con la UI dell'inventario) |

## Struttura del progetto

```
Assets/
├── Scripts/
│   ├── Items/
│   │   ├── ItemData.cs              # ScriptableObject astratto, base di ogni item
│   │   ├── HealthPotionItemData.cs  # Item concreto #1: cura chi lo usa
│   │   └── SpeedBoostItemData.cs    # Item concreto #2: moltiplicatore di velocità temporaneo
│   ├── Core/
│   │   ├── InventorySlot.cs         # Classe dati pura: item + quantità per slot
│   │   └── Inventory.cs             # Logica add/remove/use/swap + evento di cambiamento
│   ├── World/
│   │   └── ItemPickup.cs            # Pickup nel mondo, si aggiunge all'Inventory del player al trigger
│   ├── Player/
│   │   ├── IHealth.cs / PlayerHealth.cs             # Contratto salute + implementazione demo
│   │   ├── ISpeedModifiable.cs / PlayerMovement.cs  # Contratto velocità + movimento/salto
│   │   └── MouseLook.cs                             # Visuale col mouse (yaw sul corpo, pitch sulla camera) + toggle cursore
│   └── UI/
│       ├── InventorySlotUI.cs       # Singolo slot: click per usare, drag & drop
│       └── InventoryUI.cs           # Griglia di slot + icona fluttuante durante il trascinamento
├── Items/       # Asset ScriptableObject degli item (Health Potion, Speed Boost)
├── Prefabs/     # Prefab dei pickup, dello slot UI e dell'icona di drag
└── Scenes/
    └── SampleScene.unity
```

## Note di design

- **`ItemData` è uno `ScriptableObject` astratto**, con ogni tipo di item come sottoclasse che ridefinisce `Use()`. Aggiungere un nuovo item in futuro significa aggiungere una nuova sottoclasse — nessuna modifica a `Inventory`, `ItemPickup` o alla UI (Open/Closed Principle).
- **Gli effetti sono disaccoppiati tramite interfacce** (`IHealth`, `ISpeedModifiable`) invece di referenziare direttamente classi concrete del player, così gli item restano riutilizzabili su qualunque GameObject che implementi il contratto richiesto.
- **`InventorySlot` è una classe dati pura**, non un `MonoBehaviour` — facile da testare, clonare e serializzare per un futuro sistema di salvataggio.
- **`Inventory` espone un evento di cambiamento** (`OnInventoryChanged`) invece di essere chiamato direttamente dalla UI (pattern Observer), così più viste potrebbero restare sincronizzate in autonomia.
- **Il drag & drop usa le interfacce native dell'`EventSystem` di Unity** (`IBeginDragHandler`, `IDragHandler`, `IDropHandler`, ecc.) — nessun plugin di terze parti, completamente self-contained.

## Licenza

Nessuna licenza specificata al momento — tutti i diritti riservati di default finché non ne viene aggiunta una.
