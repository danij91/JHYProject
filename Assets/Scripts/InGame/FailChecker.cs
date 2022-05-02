using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FailChecker : PoolingObject {
    private Collider collider;

    private void OnTriggerEnter(Collider other) {
        GameManager.Instance.OnFail();
    }

    internal override void OnInitialize(params object[] parameters) { }

    protected override void OnUse() { }

    protected override void OnRestore() { }
}
