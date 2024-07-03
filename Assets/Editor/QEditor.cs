using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Markers;
[CustomEditor(typeof(QMarker)), CanEditMultipleObjects]
public class QEditor : Editor
{
    SerializedProperty qtype;
    // Material only
    SerializedProperty unique;
    SerializedProperty flicker;
    // Light & material
    SerializedProperty material;
    SerializedProperty changer;
    // Light only
    SerializedProperty lights;
    SerializedProperty childrenOf;
    SerializedProperty descendantsOf;
    SerializedProperty relativeIntensity;
    // mech only
    SerializedProperty cycler;
    SerializedProperty scaler;
    // Progress
    SerializedProperty progress;
    // Control
    SerializedProperty controls;
    // all alter
    SerializedProperty cycle;
    // mat alter
    SerializedProperty initialMaterial;
    SerializedProperty materials;
    // obj alter
    SerializedProperty syncWith;
    SerializedProperty initialObject;
    SerializedProperty moveTo;
    SerializedProperty objects;
    // info
    SerializedProperty detached ;
    SerializedProperty appearOnce;
    SerializedProperty colorSetting;
    SerializedProperty position;
    SerializedProperty frames;
    SerializedProperty pageControl;
    SerializedProperty areas;
    SerializedProperty references;
    SerializedProperty inLineImages;
    SerializedProperty font;
    void OnEnable()
    {
    
        qtype = serializedObject.FindProperty("qtype");
        unique = serializedObject.FindProperty("unique");
        material = serializedObject.FindProperty("material");
        flicker = serializedObject.FindProperty("flicker");
        changer = serializedObject.FindProperty("changer");
        lights = serializedObject.FindProperty("lights");
        childrenOf = serializedObject.FindProperty("childrenOf");
        descendantsOf = serializedObject.FindProperty("descendantsOf");
        relativeIntensity = serializedObject.FindProperty("relativeIntensity");
        progress = serializedObject.FindProperty("progress");
        controls = serializedObject.FindProperty("controls");
        cycler = serializedObject.FindProperty("cycler");
        scaler = serializedObject.FindProperty("scaler");

        cycle = serializedObject.FindProperty("cycle");
        initialMaterial = serializedObject.FindProperty("initialMaterial");
        materials = serializedObject.FindProperty("materials");
        syncWith = serializedObject.FindProperty("syncWith");
        initialObject = serializedObject.FindProperty("initialObject");
        moveTo = serializedObject.FindProperty("moveTo");
        objects = serializedObject.FindProperty("objects");

        detached = serializedObject.FindProperty("detached");
        appearOnce = serializedObject.FindProperty("appearOnce");
        colorSetting = serializedObject.FindProperty("colorSetting");
        position = serializedObject.FindProperty("position");
        frames = serializedObject.FindProperty("frames");
        pageControl = serializedObject.FindProperty("pageControl");
        areas = serializedObject.FindProperty("areas");
        references = serializedObject.FindProperty("references");
        inLineImages = serializedObject.FindProperty("inLineImages");
        font = serializedObject.FindProperty("font");


    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        QMarker qm = (QMarker)target;
        EditorGUILayout.PropertyField(qtype);
        switch (qm.qtype)
        {
            case QType.Material:
                EditorGUILayout.PropertyField(unique);
                EditorGUILayout.PropertyField(material);
                EditorGUILayout.PropertyField(changer);
                EditorGUILayout.PropertyField(flicker);
                break;
            case QType.Light:
                EditorGUILayout.PropertyField(material);
                EditorGUILayout.PropertyField(relativeIntensity);
                EditorGUILayout.PropertyField(lights);
                EditorGUILayout.PropertyField(childrenOf);
                EditorGUILayout.PropertyField(descendantsOf);
                EditorGUILayout.PropertyField(changer);
                EditorGUILayout.PropertyField(flicker);
                break;
            case QType.Mechanical:
                EditorGUILayout.PropertyField(cycler);
                EditorGUILayout.PropertyField(scaler);
                break;
            case QType.AlterObject:
                EditorGUILayout.PropertyField(cycle);
                EditorGUILayout.PropertyField(syncWith);
                EditorGUILayout.PropertyField(initialObject);
                EditorGUILayout.PropertyField(moveTo);
                EditorGUILayout.PropertyField(objects);
                break;
            case QType.AlterMaterial:
                EditorGUILayout.PropertyField(material);
                EditorGUILayout.PropertyField(cycle);
                EditorGUILayout.PropertyField(initialMaterial);
                EditorGUILayout.PropertyField(materials);
                break;
            case QType.Info:
                EditorGUILayout.PropertyField(detached);
                EditorGUILayout.PropertyField(appearOnce);
                EditorGUILayout.PropertyField(colorSetting);
                EditorGUILayout.PropertyField(position);
                EditorGUILayout.PropertyField(frames);
                EditorGUILayout.PropertyField(pageControl);
                EditorGUILayout.PropertyField(areas);
                EditorGUILayout.PropertyField(references);
                EditorGUILayout.PropertyField(inLineImages);
                EditorGUILayout.PropertyField(font);

                break;

        }
        if (qm.qtype != QType.AlterObject && qm.qtype != QType.AlterMaterial && qm.qtype!=QType.Info)
            EditorGUILayout.PropertyField(progress);
        EditorGUILayout.PropertyField(controls);
        serializedObject.ApplyModifiedProperties();
    }
}
