using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public Transform groundPrefab;
    public Transform nodePrefab;
    public Text statusText;
    public new Transform camera;
    public int startingCash = 200;
    public int cashPerKill = 5;
    public int lossPerEscape = 5;
    private EnemyManager enemyManager;
    private int numEnemiesDestroyed;
    private int numEnemiesEscaped;
    private CashManager cashManager;
    private CameraPositioner cameraPositioner;
    private MapFileProcessor.MapDescription map;

    private void Start() {
        cashManager = new CashManager(startingCash);
        map = MapFileProcessor.CreateMapDescription("Level1");
        CreateNodes();
        CreateGround();
        cameraPositioner = GetComponent<CameraPositioner>();
        PositionCamera();
        SetUpEnemyManager();
        cashManager.AddChangeListener(UpdateStatusText);
        UpdateStatusText();
    }

    public void ChangeView() {
        cameraPositioner.ChangeView();
    }

    private void CreateGround() {
        var ground = Instantiate(groundPrefab);
        ground.transform.localScale += Vector3.right * (map.Dimensions.x - 1) + Vector3.forward * (map.Dimensions.y - 1);
        ground.position += Vector3.right * (map.Dimensions.x / 2f - .5f) + Vector3.forward * (map.Dimensions.y / 2f - .5f);
    }

    private void SetUpEnemyManager() {
        enemyManager = GetComponent<EnemyManager>();
        enemyManager.StartPosition = map.StartPosition;
        enemyManager.Waypoints = map.Waypoints;
        enemyManager.AddChangeListener(OnEnemiesChange);
    }

    private void UpdateStatusText() => statusText.text = 
        $"Wave: <b>{enemyManager.WaveNumber}</b>, Destroyed: <b>{numEnemiesDestroyed}</b>, Escaped: <b>{numEnemiesEscaped}</b>, $<b>{cashManager.Dollars}</b>";

    private void OnEnemiesChange(EnemiesChangeEvent enemiesChangeEvent) {
        switch (enemiesChangeEvent) {
            case EnemyDestroyed ed:
                ++numEnemiesDestroyed;
                cashManager.Receive(cashPerKill);
                break;
            case EnemyEscaped ee:
                ++numEnemiesEscaped;
                cashManager.Receive(-lossPerEscape);
                break;
        }

        UpdateStatusText();
    }

    private void PositionCamera() {
        var p = camera.position;
        cameraPositioner.NormalPosition = camera.position = new Vector3(map.Dimensions.x / 2f, p.y, p.z);
        cameraPositioner.NormalRotation = camera.rotation;
    }

    private void CreateNodes() {
        var nodesParentObject = transform.Find("/Nodes");
        var nodes = map.NodePositions.Select(nodePos => Instantiate(nodePrefab, nodePos, nodePrefab.rotation));
        
        foreach (var node in nodes) {
            node.SetParent(nodesParentObject);
            node.GetComponent<Node>().CashManager = cashManager;
        }
    }
}
