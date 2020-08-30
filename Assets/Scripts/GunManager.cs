using System.Linq;
using TMPro;
using UnityEngine;

public class GunManager : MonoBehaviour {
    [SerializeField] private Gun[] guns;
    [SerializeField] private TMP_Dropdown gunsDropdown;
    public int selectedGunIndex { get ; private set; }

    private void Start() {
        gunsDropdown.options = guns.Select(gun => new TMP_Dropdown.OptionData(gun.name)).ToList();
        
        gunsDropdown.onValueChanged.AddListener(delegate (int gunIndex) {
            selectedGunIndex = gunIndex;
        });
    }
}
