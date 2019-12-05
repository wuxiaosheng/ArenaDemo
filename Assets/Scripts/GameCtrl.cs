using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCtrl : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerCtrl _player;
    private LauncherCtrl _launcher;
    void Start()
    {
        _player = new PlayerCtrl();
        _launcher = new LauncherCtrl();
        
    }

    // Update is called once per frame
    void Update()
    {
        dispatcherKeyDownEvt();

        _player.update();

        EventManager.getInstance().update();
    }

    void dispatcherKeyDownEvt() {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool isDJ = Input.GetKeyDown(KeyCode.J);
        bool isUW = Input.GetKeyUp(KeyCode.W);
        bool isUS = Input.GetKeyUp(KeyCode.S);
        bool isUA = Input.GetKeyUp(KeyCode.A);
        bool isUD = Input.GetKeyUp(KeyCode.D);
        if (h != 0 || v != 0) {
            Event evt = EventManager.getInstance().createEvent(EEventType.EVT_MOVE);
            EventManager.getInstance().trigger(evt);
        } else if (isDJ) {
            Event evt = EventManager.getInstance().createEvent(EEventType.EVT_ATK);
            EventManager.getInstance().trigger(evt);
        } else {
            Event evt = EventManager.getInstance().createEvent(EEventType.EVT_IDLE);
            EventManager.getInstance().trigger(evt);
        }
        

    }

    void OnCollisionEnter(Collision collision)//碰撞检测
    {
        //collision.collider  获取碰撞到的游戏物体身上Collider组件
        string name = collision.collider.name;  //获取碰撞到的游戏物体的名字
        Debug.Log(name);
    }

    void OnTriggerEnter(Collider collider)//触发检测
    {
        Debug.Log(collider.tag);
    }
}
