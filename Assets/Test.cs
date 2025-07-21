using System.Collections;
using UnityEngine;


/*


1. Short:
    [=] Execute: Click

2. Held:
    [=] Execute: Start holding
    [=] Finish: End holding
    [~] Cancel: RMB / Esc

3. Tool quick use:
    [=] Execute: Press shortcut
    [=] Finish: LMB / Enter
    [=] Cancel: RMB / Esc


*/


public class Test : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Start");
        Coroutine testCoroutine = StartCoroutine(test());
        if (testCoroutine == null) { Debug.Log("Null"); }
        Debug.Log("End");
    }

    private IEnumerator test()
    {
        Debug.Log("Coroutine1");
        yield return null;

        Debug.Log("Coroutine2");
    }
}
