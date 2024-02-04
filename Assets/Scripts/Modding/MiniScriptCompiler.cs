using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Miniscript;

public class MiniScriptCompiler : MonoBehaviour
{
    public Interpreter interpreter;
    static bool intrinsicsAdded = false; 
    public TextAsset script;

    void Awake() {
        AddIntrinsics();
        interpreter = new Interpreter();
    }

    void AddIntrinsics() {
        if (intrinsicsAdded) return;
        intrinsicsAdded = true;

        Intrinsic f;
        f = Intrinsic.Create("RandomMove");
        f.code = (context, partialResult) =>
        {
            transform.position = new Vector3(Random.Range(19, 24), 0, Random.Range(22, 24));
            return new Intrinsic.Result(1);
        };
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
        catch (MiniscriptException err)
        {
            Debug.Log("MiniScript Exception: " + err);
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E))
            Compile();

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
