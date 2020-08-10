public abstract class EnemiesChangeEvent { }

public class EnemyDestroyed : EnemiesChangeEvent {
    public Enemy enemy { get; }

    public EnemyDestroyed(Enemy enemy) {
        this.enemy = enemy;
    }
}

public class EnemyEscaped : EnemiesChangeEvent { }

public class WaveStarted : EnemiesChangeEvent { }

public class AllWavesCompleted : EnemiesChangeEvent { }
