using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InventorySystem.Player
{
    /// <summary>
    /// Controller di movimento dimostrativo minimale che implementa ISpeedModifiable,
    /// cosi' SpeedBoostItemData ha subito qualcosa di concreto su cui agire. Sostituitelo
    /// con il vostro character controller reale: basta implementare ISpeedModifiable
    /// e l'item continuera' a funzionare senza modifiche.
    ///
    /// Nota sull'input: questo progetto ha "Active Input Handling" impostato su
    /// "Input System Package (New)" (vedi Project Settings > Player), quindi la vecchia
    /// classe UnityEngine.Input non e' utilizzabile e lancerebbe un'eccezione a runtime.
    /// Leggiamo la tastiera direttamente da Keyboard.current: e' il modo piu' semplice per
    /// una demo self-contained, senza dover creare/collegare un InputActionAsset dedicato.
    ///
    /// Nota sul salto: CharacterController.SimpleMove (usato in precedenza) applica la
    /// gravita' internamente ma ricalcola la velocita' verticale ad ogni chiamata in base
    /// al solo stato "a terra/non a terra": non e' possibile farci convivere un impulso di
    /// salto che deve mantenersi nei frame successivi. Per questo il movimento ora usa
    /// CharacterController.Move con una velocita' verticale gestita manualmente (gravita'
    /// + salto), lo standard per questo genere di controller in Unity.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour, ISpeedModifiable
    {
        [SerializeField] private float baseSpeed = 5f;

        [Header("Salto e gravita'")]
        [SerializeField] private float jumpHeight = 1.5f;   // altezza massima raggiunta dal salto, in metri
        [SerializeField] private float gravity = -20f;       // piu' negativa di Physics.gravity.y (-9.81) per un salto piu' "arcade" e reattivo
        [SerializeField] private float groundedStickForce = -2f; // piccola spinta verso il basso mentre si e' a terra, per tenere il CharacterController "agganciato" al suolo tra un frame e l'altro

        private float currentMultiplier = 1f;
        private Coroutine activeBoostRoutine;
        private CharacterController controller;
        private float verticalVelocity;

        private void Awake() => controller = GetComponent<CharacterController>();

        private void Update()
        {
            Vector2 input = Vector2.zero;
            bool jumpPressed = false;
            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) input.y += 1f;
                if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) input.y -= 1f;
                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) input.x += 1f;
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) input.x -= 1f;
                jumpPressed = keyboard.spaceKey.wasPressedThisFrame;
            }

            // Movimento orizzontale relativo all'orientamento del player (non agli assi fissi del
            // mondo): transform.forward/right ruotano insieme al player quando MouseLook applica lo
            // yaw, cosi' "W" significa sempre "vai dove sto guardando", come in un controller FPS/TPS standard.
            Vector3 horizontalMove = transform.right * input.x + transform.forward * input.y;
            if (horizontalMove.sqrMagnitude > 1f) horizontalMove.Normalize();

            if (controller.isGrounded)
            {
                // A terra: azzeriamo la velocita' di caduta accumulata (altrimenti si sommerebbe
                // all'infinito mentre si cammina) mantenendo una piccola spinta costante verso il
                // basso, che evita al CharacterController di "galleggiare" per un frame sui dislivelli.
                if (verticalVelocity < 0f) verticalVelocity = groundedStickForce;

                if (jumpPressed)
                {
                    // v = sqrt(altezza desiderata * -2 * gravita'): formula cinematica standard per
                    // ottenere l'impulso iniziale che, frenato dalla gravita', raggiunge esattamente jumpHeight.
                    verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                }
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;
            }

            Vector3 velocity = horizontalMove * (baseSpeed * currentMultiplier) + Vector3.up * verticalVelocity;
            controller.Move(velocity * Time.deltaTime); // Move (a differenza di SimpleMove) vuole uno spostamento gia' scalato per deltaTime
        }

        public void ApplySpeedMultiplier(float multiplier, float duration)
        {
            // Se un boost e' gia' attivo lo interrompiamo e ripartiamo da capo,
            // cosi' l'uso di piu' pozioni in sequenza non fa scadere l'effetto in anticipo.
            if (activeBoostRoutine != null) StopCoroutine(activeBoostRoutine);
            activeBoostRoutine = StartCoroutine(SpeedBoostRoutine(multiplier, duration));
        }

        private IEnumerator SpeedBoostRoutine(float multiplier, float duration)
        {
            currentMultiplier = multiplier;
            Debug.Log($"[PlayerMovement] Speed boost x{multiplier} per {duration}s");
            yield return new WaitForSeconds(duration);
            currentMultiplier = 1f;
            activeBoostRoutine = null;
            Debug.Log("[PlayerMovement] Speed boost terminato");
        }
    }
}
