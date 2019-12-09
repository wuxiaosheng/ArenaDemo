
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LauncherCtrl : Object {
    private GameObject _launcher;
    public LauncherCtrl() {
        
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
        pos.y -= 0.5f;
        Event evt = EventManager.getInstance().createEvent(EEventType.EVT_BUFF, "pos", pos);
        EventManager.getInstance().trigger(evt);
        
    }

    private void onEvtLauncher(IEvent evt) {
        //随机选择一个柱子 发射一个状态
        int index = Random.Range(0,5);
        GameObject obj = _objs[index];
        launcher(obj);
    }
}