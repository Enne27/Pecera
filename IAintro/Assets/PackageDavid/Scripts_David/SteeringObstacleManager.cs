using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)] // Por defecto es cero, así que podemos forzar a que se ejecute antes.
public class SteeringObstacleManager : MonoBehaviour {
    #region Singleton
    static SteeringObstacleManager steeringObstacleManager;
    public static SteeringObstacleManager instance {
        get {
            return RequestSteeringObstacleManager();
        }
    }

    private static SteeringObstacleManager RequestSteeringObstacleManager() {
        if (!steeringObstacleManager) {
            steeringObstacleManager = FindFirstObjectByType<SteeringObstacleManager>();
        }
        return steeringObstacleManager;
    }

    private void Awake() {
        if (steeringObstacleManager == null) {
            steeringObstacleManager = this;
            steeringObstacleManager.obstacles = new List<SteeringObstacle>();
            //DontDestroyOnLoad(gameObject);

        } else if (steeringObstacleManager != this) {
            Destroy(gameObject);
        }
    }
    #endregion

    private void OnDestroy()
    {
        if(steeringObstacleManager == this) steeringObstacleManager=null;
    }

    public List<SteeringObstacle> obstacles = new List<SteeringObstacle>();

    public void RegisterObstacle(SteeringObstacle o) {
        obstacles.Add(o);
    }

    public void UnRegisterObstacle(SteeringObstacle o) {
        obstacles.Remove(o);
    }

}
