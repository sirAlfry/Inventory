# Inventory System — Unity Programming Test

A first-person Unity prototype built for a "Programming Test for Unity Developer": a fully working inventory system where the player can pick up, view, use, and drag-and-drop items in a first-person 3D scene.

## Features

- **Pickup → Inventory**: walk into world items to collect them; stackable quantities are merged automatically.
- **Two distinct item types**: a Health Potion (heals on use) and a Speed Boost (temporary movement multiplier), each with its own effect.
- **Click to use**: left-click a slot to consume/activate its item.
- **Drag & drop**: reorganize the inventory grid by dragging slots onto each other; dragging onto a matching stack merges it.
- **First-person controller**: WASD/arrow movement relative to view direction, mouse-look (yaw + pitch), and jump.
- **Cursor toggle**: `Esc` switches between mouse-look (cursor locked) and inventory interaction (cursor free).

## Tech stack

| | |
|---|---|
| Engine | Unity `6000.4.0f1` |
| Render pipeline | Universal Render Pipeline (URP) |
| Input | Input System package (new), no legacy `UnityEngine.Input` |
| Language | C# |
| UI | Unity UGUI (`Canvas` / `Text`) |

## Getting started

1. Clone the repository:
   ```bash
   git clone https://github.com/sirAlfry/Inventory.git
   ```
2. Open the project with **Unity Hub**, version `6000.4.0f1` (or a compatible Unity 6 release).
3. Open `Assets/Scenes/SampleScene.unity`.
4. Press **Play**.

No external packages or paid assets are required beyond what ships with the project (`Packages/manifest.json`), which includes the Input System and URP.

## Controls

| Input | Action |
|---|---|
| `W` `A` `S` `D` / Arrow keys | Move (relative to where you're looking) |
| Mouse | Look around (horizontal + vertical) |
| `Space` | Jump (grounded only) |
| Left click on a slot | Use the item in that slot |
| Drag a slot | Move/swap/merge items in the inventory |
| `Esc` | Toggle cursor lock (needed to interact with the inventory UI) |

## Project structure

```
Assets/
├── Scripts/
│   ├── Items/
│   │   ├── ItemData.cs              # Abstract ScriptableObject base for every item
│   │   ├── HealthPotionItemData.cs  # Concrete item #1: heals the user
│   │   └── SpeedBoostItemData.cs    # Concrete item #2: temporary speed multiplier
│   ├── Core/
│   │   ├── InventorySlot.cs         # Plain data class: item + quantity per slot
│   │   └── Inventory.cs             # Add/remove/use/swap logic + change event
│   ├── World/
│   │   └── ItemPickup.cs            # World pickup, adds itself to the player's Inventory on trigger
│   ├── Player/
│   │   ├── IHealth.cs / PlayerHealth.cs             # Health contract + demo implementation
│   │   ├── ISpeedModifiable.cs / PlayerMovement.cs  # Speed contract + movement/jump implementation
│   │   └── MouseLook.cs                             # Mouse-look (yaw on the body, pitch on the camera) + cursor toggle
│   └── UI/
│       ├── InventorySlotUI.cs       # Single slot: click-to-use, drag & drop
│       └── InventoryUI.cs           # Slot grid + floating drag icon
├── Items/       # ItemData ScriptableObject assets (Health Potion, Speed Boost)
├── Prefabs/     # Pickup prefabs, UI slot prefab, drag icon prefab
└── Scenes/
    └── SampleScene.unity
```

## Design notes

- **`ItemData` is an abstract `ScriptableObject`**, with each item type as a subclass overriding `Use()`. Adding a new item later means adding a new subclass — no changes needed to `Inventory`, `ItemPickup`, or the UI (Open/Closed Principle).
- **Effects are decoupled through interfaces** (`IHealth`, `ISpeedModifiable`) rather than referencing concrete player classes directly, so items stay reusable on any GameObject that implements the relevant contract.
- **`InventorySlot` is a plain data class**, not a `MonoBehaviour` — easy to test, clone, and serialize for a future save system.
- **`Inventory` exposes a change event** (`OnInventoryChanged`) instead of being called directly by the UI (Observer pattern), so multiple views could stay in sync independently.
- **Drag & drop uses Unity's built-in `EventSystem` interfaces** (`IBeginDragHandler`, `IDragHandler`, `IDropHandler`, etc.) — no third-party plugin, fully self-contained.

## License

No license specified yet — all rights reserved by default until one is added.
