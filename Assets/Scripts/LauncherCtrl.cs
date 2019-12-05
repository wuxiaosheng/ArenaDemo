
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LauncherCtrl : Object {
    private List<GameObject> _objs;
    public LauncherCtrl() {
        _objs = new List<GameObject>();
        for (int i = 1; i < 6; i++) {
            _objs.Add(GameObject.Find("launcher"+i));
        }
        onAddListener();
    }

    public void onAddListener() {
        EventManager.getInstance().addEventListener(EEventType.EVT_LAUNCHER, new Handler(onEvtLauncher));
    }

    public void onRemoveListener() {
        EventManager.getInstance().removeEventListener(EEventType.EVT_LAUNCHER);
    }

    public void update() {

    }

    public void launcher(GameObject obj) {
        Transform child = obj.transform.Find("Sphere");
        Vector3 pos = child.position;
        //child.localToWorldMatrix
        
        
    }

    private void onEvtLauncher(IEvent evt) {
        //随机选择一个柱子 发射一个状态
        int index = Random.Range(0,5);
        GameObject obj = _objs[index];
        launcher(obj);
    }
}