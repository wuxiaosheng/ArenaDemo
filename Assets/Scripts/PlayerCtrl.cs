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
    private float _speed;
    private bool _isReverse;
    private float _buffTime;
    private float _atkCD;
    private float _bigTime;
    public PlayerCtrl() {
        _parent = GameObject.Find("GameObject");
        _obj = Instantiate((GameObject)Resources.Load("Charactor/TT_demo/prefabs/TT_demo_male_A"));
        _obj.transform.SetParent(_parent.transform);
        _obj.transform.position = new Vector3(0.0f, 0.85f, 6.5f);
        _ani = _obj.GetComponent<Animator>();
        _status = 0;
        _hp = 100;
        _atkCD = 0;
        _buffTime = 0.0f;
        initAttr();
        onAddListener();
    }

    private void initAttr() {
        
        _atk = 5;
        _def = 4;
        _speed = 1;
        _isReverse = false;
        _buffTime = 0.0f;
        _obj.GetComponent<Renderer>().material.color = Color.red;
    }

    public void onAddListener() {
        EventManager.getInstance().addEventListener(EEventType.EVT_MOVE, new Handler(onEvtMove));
        EventManager.getInstance().addEventListener(EEventType.EVT_IDLE, new Handler(onEvtIdle));
        EventManager.getInstance().addEventListener(EEventType.EVT_ATK, new Handler(onEvtAtk));
        EventManager.getInstance().addEventListener(EEventType.EVT_PLAYER_EAT_BUFF, new Handler(onEvtUseBuff));
        EventManager.getInstance().addEventListener(EEventType.EVT_PLAYER_HURT, new Handler(onEvtHurt));
    }

    public void onRemoveListener() {
        EventManager.getInstance().removeEventListener(EEventType.EVT_MOVE);
        EventManager.getInstance().removeEventListener(EEventType.EVT_IDLE);
        EventManager.getInstance().removeEventListener(EEventType.EVT_ATK);
        EventManager.getInstance().removeEventListener(EEventType.EVT_PLAYER_EAT_BUFF);
        EventManager.getInstance().removeEventListener(EEventType.EVT_PLAYER_HURT);
    }

    private void onEvtMove(IEvent evt) {
        if (isAtkCD()) { return; }
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (h==0.0f && v== 0.0f) { return; }
        Vector3 vec3 = _obj.transform.position;
        float hVal = (h*0.1f*_speed*(_isReverse?-1:1));
        float vVal = (v*0.1f*_speed*(_isReverse?-1:1));
        _obj.transform.position = new Vector3(vec3.x+hVal, vec3.y, vec3.z+vVal);
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
        this.setAtkCD();
        setStatus(2);
    }

    void onEvtUseBuff(IEvent evt) {
        BuffInfo info = (BuffInfo)evt.getArg("info");
        if (info._type == 0) {
            _atk += info._atk;
        } else if (info._type == 1) {
            _hp += info._hp;
        } else if (info._type == 2) {
            _def += info._def;
        } else if (info._type == 3) {
            _speed *= info._speed;
        } else if (info._type == 4) {
            _isReverse = info._isReverse;
        }
        _buffTime = info._existTime/10.0f*5.0f;
        
    }

    void onEvtHurt(IEvent evt) {
        Vector3 vec3 = _obj.transform.position;
        float num = ((float)evt.getArg("hurtNum")-_def);
        num = (num < 0.0f ? 0.0f : num);
        this.setHP(_hp-num);
        //_obj.transform.position = new Vector3(vec3.x, vec3.y, vec3.z-0.7f);
        _obj.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        _bigTime = 0.05f;
        Debug.Log("player hp:"+_hp);
    }

    public void onEvtDead() {
        setStatus(3);
        onRemoveListener();
    }

    public void update() {
        if (_buffTime > 0.0f) {
            _buffTime -= Time.deltaTime;
        } else {
            initAttr();
        }

        if (_atkCD > 0.0f) {
            _atkCD -= Time.deltaTime;
        } else {
            _atkCD = 0.0f;
        }

        if (_bigTime <= 0.0f) {
            _obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        } else {
            _bigTime -= Time.deltaTime;
        }

        Quaternion rotationInfo = _obj.transform.rotation;
        _obj.transform.rotation = new Quaternion(0.0f, rotationInfo.y, 0.0f, rotationInfo.w);
    }

    private void setStatus(int status) {
        if (status == _status) { return; }
        _status = status;
        _ani.SetInteger("PlayerAniStatus", status);
    }

    public int getStatus() {
        return _status;
    }

    public Vector3 getPosition() {
        return _obj.transform.position;
    }

    public float getViewMin() {
        return _obj.transform.rotation.y*180.0f-45.0f;
    }

    public float getViewMax() {
        return _obj.transform.rotation.y*180.0f+45.0f;
    }

    public Transform getTransform() {
        return _obj.transform;
    } 

    public void setHP(float hp) {
        _hp = hp;
    }

    public float getAtk() {
        return _atk;
    }

    public float getHP() {
        return _hp;
    }

    public bool isAtkCD() {
        return _atkCD != 0.0f;
    }

    public void setAtkCD() {
        _atkCD = 0.3f;
    }
}