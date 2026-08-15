using UnityEngine;
using UnityEngine.InputSystem;

namespace InventorySystem.Player
{
    /// <summary>
    /// Controllo della visuale col mouse, in stile FPS/TPS standard:
    ///  - il movimento orizzontale del mouse (asse X) ruota il CORPO del player (yaw). Dato che
    ///    PlayerMovement ora si muove in base a transform.forward/transform.right (vedi le
    ///    modifiche a quello script), la direzione di movimento segue automaticamente lo sguardo,
    ///    senza bisogno di sincronizzare i due script a mano;
    ///  - il movimento verticale del mouse (asse Y) ruota SOLO la camera (pitch), clampato per
    ///    evitare capriole all'indietro.
    ///
    /// Setup: componente da mettere sul GameObject Player. Nel campo "Camera Transform" trascina
    /// la Main Camera, che deve essere FIGLIA di Player (vedi README) cosi' eredita automaticamente
    /// lo yaw del corpo tramite la gerarchia; il pitch invece viene applicato qui direttamente
    /// alla sua rotazione locale.
    ///
    /// Nota UI: mentre il cursore e' "agganciato" (lock) per guardarsi intorno, il sistema
    /// operativo non lo rende visibile ne' cliccabile, quindi non potresti interagire con gli slot
    /// dell'inventario (click per usare, drag & drop). Per questo il tasto Escape sblocca/riblocca
    /// il cursore: sbloccalo prima di aprire l'inventario, poi ribloccalo per tornare a muovere la visuale.
    /// </summary>
    public class MouseLook : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float mouseSensitivity = 120f; // gradi/secondo per unita' di delta del mouse
        [SerializeField] private float minPitch = -80f;
        [SerializeField] private float maxPitch = 80f;

        private float yaw;
        private float pitch;
        private bool cursorLocked = true;

        private void Start()
        {
            // Inizializza yaw/pitch dalla rotazione gia' impostata in editor, cosi' non "scatta"
            // di colpo al primo frame se la scena e' stata orientata manualmente.
            yaw = transform.eulerAngles.y;
            if (cameraTransform != null)
                pitch = cameraTransform.localEulerAngles.x;

            SetCursorLocked(true);
        }

        private void Update()
        {
            HandleCursorToggle();
            if (!cursorLocked) return; // cursore libero (es. per usare l'inventario): non ruotare la visuale

            var mouse = Mouse.current;
            if (mouse == null) return;

            Vector2 delta = mouse.delta.ReadValue() * (mouseSensitivity * Time.deltaTime);

            yaw += delta.x;
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);

            pitch = Mathf.Clamp(pitch - delta.y, minPitch, maxPitch);
            if (cameraTransform != null)
                cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        private void HandleCursorToggle()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                SetCursorLocked(!cursorLocked);
        }

        private void SetCursorLocked(bool locked)
        {
            cursorLocked = locked;
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}
