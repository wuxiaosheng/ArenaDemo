using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BuffInfo {
    public int _hp;
    public int _atk;
    public int _def;
    public float _speed;
    public bool _isReverse = false;
    public float _existTime;
    public int _type;
}
public class BuffCtrl : Object {
    private GameObject _parent;
    private List<KeyValuePair<GameObject, BuffInfo>> _list;
    public BuffCtrl() {
        _list = new List<KeyValuePair<GameObject, BuffInfo>>();
        _parent = GameObject.Find("GameObject");
        onAddListener();
    }

    public void onAddListener() {
        EventManager.getInstance().addEventListener(EEventType.EVT_USE_BUFF, new Handler(onEvtUseBuff));
    }

    public void onRemoveListener() {
        EventManager.getInstance().removeEventListener(EEventType.EVT_USE_BUFF);
    }

    public void createBuff(Vector3 pos) {
        GameObject buff = Instantiate((GameObject)Resources.Load("Buff"));
        buff.transform.localPosition = new Vector3(pos.x, pos.y, pos.z+0.5f);
        buff.transform.SetParent(_parent.transform);
        initBuffInfo(buff, Random.Range(0, 5));
    }

    private void initBuffInfo(GameObject buff, int type) {
        BuffInfo info = new BuffInfo();
        info._isReverse = false;
        if (type == 0) {
            info._atk = Random.Range(-9, 10);
            buff.GetComponent<Renderer>().material.color = Color.red;
        } else if (type == 1) {
            info._hp = Random.Range(-19, 20);
            buff.GetComponent<Renderer>().material.color = Color.green;
        } else if (type == 2) {
            info._def = Random.Range(-4, 5);
            buff.GetComponent<Renderer>().material.color = Color.blue;
        } else if (type == 3) {
            info._speed = Random.Range(-10, 11)/10.0f;
            buff.GetComponent<Renderer>().material.color = Color.black;
        } else if (type == 4) {
            info._isReverse = (Random.Range(0, 2) == 0);
            buff.GetComponent<Renderer>().material.color = Color.grey;
        }
        info._type = type;
        info._existTime = 15.0f;
        _list.Add(new KeyValuePair<GameObject, BuffInfo>(buff, info));
        Vector3 force = (new Vector3(0.0f, buff.transform.position.y, 0.0f)-buff.transform.position)*100.0f;
        buff.GetComponent<Rigidbody>().AddForce(force);
    }

    public GameObject getNearbyObj(Vector3 pos) {
        for (int i = 0; i < _list.Count; i++) {
            float distance = Vector3.Distance(_list[i].Key.transform.position, pos);
            if (distance < 1.3f) {
                return _list[i].Key;
            }
        }

        return null;
    }

    public BuffInfo GetBuffInfo(GameObject obj) {
        for (int i = 0; i < _list.Count; i++) {
            if (_list[i].Key == obj) {
                return _list[i].Value;
            }
        }
        return null;
    }

    public void update() {
        for (int i = _list.Count-1; i >= 0; i--) {
            KeyValuePair<GameObject, BuffInfo> pair = _list[i];
            pair.Value._existTime -= Time.deltaTime;
            KeyValuePair<GameObject, BuffInfo> temp = new KeyValuePair<GameObject, BuffInfo>(pair.Key, pair.Value);
            _list.RemoveAt(i);
            if (temp.Value._existTime > 0.0f) {
                _list.Add(temp);
            } else {
                Destroy(temp.Key);
            }
        }
    }

    void onEvtUseBuff(IEvent evt) {
        GameObject obj = (GameObject)evt.getArg("obj");

        for (int i = _list.Count-1; i >= 0; i--) {
            KeyValuePair<GameObject, BuffInfo> pair = _list[i];
            if (pair.Key == obj) {
                Destroy(obj);
                _list.RemoveAt(i);
                break;
            }
        }
    }
}
