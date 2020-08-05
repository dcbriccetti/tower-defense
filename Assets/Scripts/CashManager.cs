using System;

public class CashManager {
    private Action changeListener;
    public int Dollars { get; private set; }

    public CashManager(int dollars) {
        Dollars = dollars;
    }

    public bool Buy(int costInDollars) {
        if (costInDollars > Dollars) return false;
        Dollars -= costInDollars;
        changeListener();
        return true;
    }

    public void Receive(int dollars) {
        Dollars += dollars;
        changeListener();
    }

    public void AddChangeListener(Action listener) {
        changeListener = listener;
    }
}
