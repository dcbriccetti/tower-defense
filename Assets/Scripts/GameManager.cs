using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public Transform groundPrefab;
    public Transform nodePrefab;
    public TextMeshProUGUI statusText;
    public GameObject gameOverParent;
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
        SetCameraNormalPosition();
        cameraPositioner = GetComponent<CameraPositioner>();
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
            case AllWavesCompleted awc:
                GameOver();
                break;
        }

        UpdateStatusText();
    }

    private void GameOver() {
        gameOverParent.SetActive(true);
    }

    public void PlayAgain() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void SetCameraNormalPosition() {
        Vector3 p = camera.position;
        camera.position = new Vector3(map.Dimensions.x / 2f, p.y, p.z);
    }

    private void CreateNodes() {
        var nodesParentObject = transform.Find("/Nodes");
        var nodeTransforms = map.NodePositions.Select(nodePos => Instantiate(nodePrefab, nodePos,
            nodePrefab.rotation, nodesParentObject));
        
        foreach (var nt in nodeTransforms) {
            Node node = nt.GetComponent<Node>();
            node.CashManager = cashManager;
            node.IsMouseClickAllowed = IsMouseClickAllowed;
        }
    }
    
    private bool IsMouseClickAllowed() => cameraPositioner.IsNormalView();
}
