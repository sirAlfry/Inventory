namespace InventorySystem.Player
{
    /// <summary>
    /// Contratto per qualsiasi entita' la cui velocita' di movimento possa essere modificata
    /// temporaneamente, ad esempio da un consumabile SpeedBoost. Il disaccoppiamento tramite
    /// interfaccia mantiene il sistema di item indipendente da una specifica implementazione
    /// di movimento (CharacterController, Rigidbody, NavMeshAgent, ecc.).
    /// </summary>
    public interface ISpeedModifiable
    {
        /// <summary>Applica un moltiplicatore di velocita' per una durata (in secondi).</summary>
        void ApplySpeedMultiplier(float multiplier, float duration);
    }
}
