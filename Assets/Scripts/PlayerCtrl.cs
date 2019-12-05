using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerCtrl : Object {
    private int _status;
    private Animator _ani;
    private GameObject _obj;
    private GameObject _parent;
    private float _hp;
    private float _atk;
    private float _def;
    private float _maxHP;
    public PlayerCtrl() {
        _parent = GameObject.Find("GameObject");
        _obj = Instantiate((GameObject)Resources.Load("Charactor/TT_demo/prefabs/TT_demo_male_A"));
        _obj.transform.parent = _parent.transform;
        _obj.transform.position = new Vector3(0.0f, 0.85f, 6.5f);
        _ani = _obj.GetComponent<Animator>();
        _status = 0;
        _hp = 100;
        _atk = 5;
        _def = 10;
        _maxHP = 100;
        onAddListener();
    }

    public void onAddListener() {
        EventManager.getInstance().addEventListener(EEventType.EVT_MOVE, new Handler(onEvtMove));
        EventManager.getInstance().addEventListener(EEventType.EVT_IDLE, new Handler(onEvtIdle));
        EventManager.getInstance().addEventListener(EEventType.EVT_ATK, new Handler(onEvtAtk));
    }

    public void onRemoveListener() {
        EventManager.getInstance().removeEventListener(EEventType.EVT_MOVE);
        EventManager.getInstance().removeEventListener(EEventType.EVT_IDLE);
        EventManager.getInstance().removeEventListener(EEventType.EVT_ATK);
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
        _obj.transform.rotation = new Quaternion(0.0f, res.x == 1 ? res.x : res.y,res.z, res.w);
        setStatus(1);
    }

    private void onEvtIdle(IEvent evt) {
        setStatus(0);
    }

    private void onEvtAtk(IEvent evt) {
        setStatus(2);
    }

    public void update() {
        Quaternion rotationInfo = _obj.transform.rotation;
        _obj.transform.rotation = new Quaternion(0.0f, rotationInfo.y, 0.0f, rotationInfo.w);
    }

    private void setStatus(int status) {
        if (status == _status) { return; }
        _status = status;
        _ani.SetInteger("PlayerAniStatus", status);
    }
}