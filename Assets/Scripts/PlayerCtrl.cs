using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerCtrl : Object {
    private GameObject _obj;
    private GameObject _parent;
    public PlayerCtrl() {
        _parent = GameObject.Find("GameObject");
        _obj = Instantiate((GameObject)Resources.Load("Charactor/TT_demo/prefabs/TT_demo_male_A"));
        _obj.transform.parent = _parent.transform;
        onAddListener();
    }

    public void onAddListener() {
        EventManager.getInstance().addEventListener(EEventType.EVT_MOVE, new Handler(onEvtMove));
        EventManager.getInstance().addEventListener(EEventType.EVT_ATK, new Handler(onEvtAtk));
    }

    public void onRemoveListener() {

    }

    private void onEvtMove(IEvent evt) {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 vec3 = _obj.transform.position;
        _obj.transform.position = new Vector3(vec3.x+h*0.1f, vec3.y, vec3.z+v*0.1f);

        //角度
        Vector3 from = Vector3.forward;
        Vector3 to = _obj.transform.position - vec3;
        Quaternion res = Quaternion.FromToRotation(from, to);
        _obj.transform.rotation = new Quaternion(0, res.y,res.z, res.w);
    }

    private void onEvtAtk(IEvent evt) {

    }

    public void update() {
        //Quaternion rotationInfo = _obj.transform.rotation;
        //_obj.transform.rotation = new Quaternion(rotationInfo.x, 1.0f, rotationInfo.z, rotationInfo.w);
    }
}