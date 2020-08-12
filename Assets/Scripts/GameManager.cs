﻿using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The top level controller for the game
/// </summary>
public class GameManager : MonoBehaviour {
    [SerializeField] private Transform groundPrefab;
    [SerializeField] private Transform nodePrefab;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private GameObject gameOverParent;
    [SerializeField] private new Transform camera;
    [SerializeField] private int startingCash = 200;
    [SerializeField] private int lossPerEscape = 5;
    private EnemyManager enemyManager;
    private int numEnemiesDestroyed;
    private int numEnemiesEscaped;
    private CashManager cashManager;
    private CameraPositioner cameraPositioner;
    private MapFileProcessor.MapDescription map;
    private int selectedGunIndex;
    private GunManager gunManager;
    
    private void Start() {
        cashManager = new CashManager(startingCash);
        gunManager = GetComponent<GunManager>();
        map = MapFileProcessor.CreateMapDescription("Level1");
        CreateNodes();
        CreateGround();
        SetCameraNormalPosition();
        cameraPositioner = GetComponent<CameraPositioner>();
        SetUpEnemyManager();
        cashManager.AddChangeListener(UpdateStatusText);
        UpdateStatusText();
    }

    /// <summary>
    /// Cycles among the enabled views
    /// </summary>
    public void ChangeView() => cameraPositioner.ChangeView();

    private void CreateGround() {
        var ground = Instantiate(groundPrefab);
        ground.transform.localScale += Vector3.right * (map.Dimensions.x - 1) + Vector3.forward * (map.Dimensions.y - 1);
        ground.position += Vector3.right * (map.Dimensions.x / 2f - .5f) + Vector3.forward * (map.Dimensions.y / 2f - .5f);
    }

    private void SetUpEnemyManager() {
        enemyManager = GetComponent<EnemyManager>();
        enemyManager.StartPosition = map.StartPosition;
        enemyManager.Waypoints = map.Waypoints;
        enemyManager.ChangeListener = OnEnemiesChange;
    }

    private void UpdateStatusText() => statusText.text = 
        $"Wave: <b>{enemyManager.WaveNumber}</b>, Destroyed: <b>{numEnemiesDestroyed}</b>, Escaped: <b>{numEnemiesEscaped}</b>, $<b>{cashManager.Dollars}</b>";

    private void OnEnemiesChange(EnemiesChangeEvent enemiesChangeEvent) {
        switch (enemiesChangeEvent) {
            case EnemyDestroyed ed:
                ++numEnemiesDestroyed;
                cashManager.Receive(ed.enemy.killValue);
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

    private static void OnNodeChangeEvent(NodeChangeEvent nodeChangeEvent) {
        switch (nodeChangeEvent) {
            case GunAddedToNode gan:
                DisableInstructions();
                break;
        }
    }

    private static void DisableInstructions() {
        foreach (var inst in GameObject.FindGameObjectsWithTag("Instructions")) inst.SetActive(false);
    }

    private void GameOver() => gameOverParent.SetActive(true);

    public void PlayAgain() => SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Todo get the button to call this again

    private void SetCameraNormalPosition() {
        Vector3 p = camera.position;
        camera.position = new Vector3(map.Dimensions.x / 2f, p.y, p.z);
    }

    private void CreateNodes() {
        var nodesParentObject = transform.Find("/Instance Containers/Nodes");
        var nodeTransforms = map.NodePositions.Select(nodePos => Instantiate(nodePrefab, nodePos,
            nodePrefab.rotation, nodesParentObject));
        
        foreach (var nt in nodeTransforms) {
            Node node = nt.GetComponent<Node>();
            node.CashManager = cashManager;
            node.IsMouseClickAllowed = IsMouseClickAllowed;
            node.SelectedGunIndexProvider = () => gunManager.selectedGunIndex;
            node.NodeChangeListener = OnNodeChangeEvent;
        }
    }
    
    private bool IsMouseClickAllowed() => cameraPositioner.IsNormalView();
}
