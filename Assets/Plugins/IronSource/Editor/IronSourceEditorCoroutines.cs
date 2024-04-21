using System.Collections;
using UnityEditor;

public class IronSourceEditorCoroutines
{
    private readonly IEnumerator mRoutine;

    private IronSourceEditorCoroutines(IEnumerator routine)
    {
        mRoutine = routine;
    }

    public static IronSourceEditorCoroutines StartEditorCoroutine(IEnumerator routine)
    {
        var coroutine = new IronSourceEditorCoroutines(routine);
        coroutine.start();
        return coroutine;
    }

    private void start()
    {
        EditorApplication.update += update;
    }

    private void update()
    {
        if (!mRoutine.MoveNext()) StopEditorCoroutine();
    }

    public void StopEditorCoroutine()
    {
        EditorApplication.update -= update;
    }
}