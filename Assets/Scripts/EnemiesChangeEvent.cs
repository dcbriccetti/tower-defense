public class EnemiesChangeEvent {
    public EnemiesChangeEvent(int numDestroyed, int numEscaped) {
        NumDestroyed = numDestroyed;
        NumEscaped = numEscaped;
    }

    public int NumDestroyed { get; }
    public int NumEscaped { get; }
}