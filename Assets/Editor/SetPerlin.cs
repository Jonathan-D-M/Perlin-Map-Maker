using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PerlinNoise))]
public class SetPerlin : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PerlinNoise thisscript = (PerlinNoise)target;
        //thisscript.loopvalx =  EditorGUILayout.IntField("X value", thisscript.loopvalx);
        //thisscript.loopvaly = EditorGUILayout.IntField("Y value", thisscript.loopvaly);

        if(GUILayout.Button("Run With New Values"))
        {
            //redo all this with the new values.
            //thisscript
            thisscript.destroyTiles();

            if(thisscript.getMultipleLayers())
            {
                thisscript.generateMultiLayerGrid(thisscript.getLayerCount());
            }
            else
            {
                thisscript.generate1LayerGrid();
            }
        }
    }


}
