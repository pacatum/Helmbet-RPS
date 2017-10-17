using System;
using System.Collections;
using UnityEngine;

public class CoroutineManager : SingletonMonoBehaviour<CoroutineManager> {

    public void StartCoroutineMethod( IEnumerator method ) {
        StartCoroutine( method );
    }
}
