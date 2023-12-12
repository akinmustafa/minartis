using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Miniscript;

public class MiniScriptCompiler : MonoBehaviour
{
    public Interpreter interpreter;
    public TextAsset script;

    void Awake() {
        interpreter = new Interpreter();
    }

    void AddIntrinsics() {
        // Empty for future intrinsics.
    }

    [ContextMenu("Compile")]
    void Compile() {
        if (interpreter != null)
            interpreter.Stop();
        interpreter = new Interpreter();
        interpreter.standardOutput = (string s, bool lineBreak) => Debug.Log("MiniScript: " + s);
        interpreter.errorOutput = (string s, bool lineBreak) => Debug.LogError("MiniScript: " + s);

        string constants = "_events=[];";
        interpreter.Reset(constants + script.text);
        interpreter.Compile();

        try
        {
            interpreter.RunUntilDone();
        }
        catch(MiniscriptException err)
        {
            Debug.Log("MiniScript Exception: " + err);
        }
    }

    private void Update() {
        if (interpreter == null || !interpreter.Running())
            return;
        try
        {
            interpreter.RunUntilDone(0.001f);
        }
        catch (MiniscriptException err)
        {
            Debug.Log("MiniScript Exception: " + err);
        }
    }
}
