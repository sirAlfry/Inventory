namespace InventorySystem.Player
{
    /// <summary>
    /// Contratto generico per qualsiasi entita' che possa subire danno o essere curata.
    /// Usare un'interfaccia (invece di referenziare direttamente una classe PlayerHealth)
    /// mantiene il sistema di inventario/item disaccoppiato da un'implementazione specifica
    /// del giocatore: qualunque GameObject che implementa IHealth puo' essere curato da
    /// una pozione, incluso ad esempio un NPC alleato in futuro.
    /// </summary>
    public interface IHealth
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }
        void Heal(float amount);
        void Damage(float amount);
    }
}
