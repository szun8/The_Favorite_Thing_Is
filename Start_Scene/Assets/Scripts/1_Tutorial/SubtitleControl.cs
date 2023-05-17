using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubtitleControl : MonoBehaviour
{
    public void RunSubtitleUI(string runAnim)
    {
        UIManager.instnace.RunAnims(runAnim);
    }
}
