using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

[CustomEditor(typeof(SteeringBehaviour))]
public class SteeringBehaviourEditor : Editor
{
    SerializedProperty currentState;
    SerializedProperty fishType;

    SerializedProperty targetSteeringBehaviour;
    SerializedProperty maxVelocity;
    SerializedProperty mass;
    SerializedProperty slowingRadius;

    SerializedProperty wanderEvaluationTime;
    SerializedProperty wanderCircleRadius;
    SerializedProperty wanderCircleDistance;
    SerializedProperty maxWanderAngle;

    SerializedProperty maxSeeAhead;
    SerializedProperty maxAvoidForce;


    SerializedProperty baseMaxVelocity;
    SerializedProperty fleeMultiplier;

    SerializedProperty rigidBody;

    bool allowEdit;

    private void OnEnable()
    {
        fishType = serializedObject.FindProperty("fishType");

        targetSteeringBehaviour = serializedObject.FindProperty("target");
        currentState = serializedObject.FindProperty("currentState");
        maxVelocity = serializedObject.FindProperty("maxVelocity");


        slowingRadius = serializedObject.FindProperty("slowingRadius");
        mass = serializedObject.FindProperty("mass");

        fleeMultiplier = serializedObject.FindProperty("fleeMultiplier");
        baseMaxVelocity = serializedObject.FindProperty("baseMaxVelocity");

        wanderCircleRadius = serializedObject.FindProperty("wanderCircleRadius");
        wanderEvaluationTime = serializedObject.FindProperty("wanderEvaluationTime");
        wanderCircleDistance = serializedObject.FindProperty("wanderCircleDistance");
        maxWanderAngle = serializedObject.FindProperty("maxWanderAngle");


        maxSeeAhead = serializedObject.FindProperty("maxSeeAhead");
        maxAvoidForce = serializedObject.FindProperty("maxAvoidForce");

        rigidBody = serializedObject.FindProperty("rb");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GUI.enabled = false;
        EditorGUILayout.PropertyField(fishType);
        GUI.enabled = true;

        allowEdit = GUILayout.Toggle(allowEdit, "Editable");
        GUI.enabled = allowEdit;
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(currentState);
        EditorGUILayout.PropertyField(rigidBody);


        EditorGUILayout.Space();
        //EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(maxVelocity);
        EditorGUILayout.PropertyField(mass);

       // EditorGUILayout.LabelField("Collision Avoidance", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(maxSeeAhead);
        EditorGUILayout.PropertyField(maxAvoidForce);

        // Mostrar campos según el estado actual
        SteeringBehaviour.FishStates state = (SteeringBehaviour.FishStates)currentState.enumValueIndex;

        EditorGUILayout.Space();
        switch (state)
        {
            case SteeringBehaviour.FishStates.SEEK:
                //EditorGUILayout.LabelField("Seek Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(targetSteeringBehaviour);
                break;

            case SteeringBehaviour.FishStates.WANDER:
                EditorGUILayout.LabelField("Wander Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(wanderCircleRadius, new GUIContent("Circle Radius"));
                EditorGUILayout.PropertyField(wanderCircleDistance, new GUIContent("Circle Distance"));
                EditorGUILayout.PropertyField(maxWanderAngle, new GUIContent("Max Angle"));
                EditorGUILayout.PropertyField(wanderEvaluationTime, new GUIContent("Evaluation Time"));
                break;

            case SteeringBehaviour.FishStates.FLEE:
                EditorGUILayout.PropertyField(targetSteeringBehaviour);
                EditorGUILayout.PropertyField(baseMaxVelocity);
                EditorGUILayout.PropertyField(fleeMultiplier);
                break;
            case SteeringBehaviour.FishStates.ARRIVE:
                    EditorGUILayout.PropertyField(slowingRadius);
                break;
        }
        GUI.enabled = true;
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
