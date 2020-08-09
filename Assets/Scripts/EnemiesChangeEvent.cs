public abstract class EnemiesChangeEvent { }

public class EnemyDestroyed : EnemiesChangeEvent { }

public class EnemyEscaped : EnemiesChangeEvent { }

public class WaveStarted : EnemiesChangeEvent { }
