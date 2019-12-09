using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCtrl : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerCtrl _player;
    private EnemyCtrl _enemy;
    private LauncherCtrl _launcher;
    private BuffCtrl _buff;
    private float _timer;
    private float _notifyPlayerHurt;
    private float _notifyEnemyHurt;
    void Start()
    {
        _player = new PlayerCtrl();
        _enemy = new EnemyCtrl();
        _launcher = new LauncherCtrl();
        _buff = new BuffCtrl();
        _timer = 10.0f;
        onAddListener();
        
    }

    // Update is called once per frame
    void Update()
    {
        _timer -= Time.deltaTime;
        dispatcherKeyDownEvt();
        checkLauncher();
        checkBuff();
        checkCollider();

        _player.update();
        _enemy.update();
        _launcher.update();
        _buff.update();

        EventManager.getInstance().update();
        
        if (_enemy.getHP() <= 0.0f) {
            _enemy.onEvtDead();
        }

        if (_player.getHP() <= 0.0f) {
            _player.onEvtDead();
        }

        if (_notifyPlayerHurt > 0.0f) {
            _notifyPlayerHurt -= Time.deltaTime;
        } else {
            _notifyPlayerHurt = 0.0f;
        }

        if (_notifyEnemyHurt > 0.0f) {
            _notifyEnemyHurt -= Time.deltaTime;
        } else {
            _notifyEnemyHurt = 0.0f;
        }
        
    }

    public void onAddListener() {
        EventManager.getInstance().addEventListener(EEventType.EVT_BUFF, new Handler(onEvtBuff));
    }

    public void onRemoveListener() {
        EventManager.getInstance().removeEventListener(EEventType.EVT_BUFF);
    }

    void dispatcherKeyDownEvt() {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool isDJ = Input.GetKeyDown(KeyCode.J);
        bool isUW = Input.GetKeyUp(KeyCode.W);
        bool isUS = Input.GetKeyUp(KeyCode.S);
        bool isUA = Input.GetKeyUp(KeyCode.A);
        bool isUD = Input.GetKeyUp(KeyCode.D);
        bool isDKEnter = Input.GetKeyDown(KeyCode.KeypadEnter);
        Event evt;
        if (isDJ && !_player.isAtkCD()) {
            evt = EventManager.getInstance().createEvent(EEventType.EVT_ATK);
            EventManager.getInstance().broadcast(evt);
        } else if ((h != 0 || v != 0)) {
            evt = EventManager.getInstance().createEvent(EEventType.EVT_MOVE);
            EventManager.getInstance().broadcast(evt);
        }
        if ((h == 0 && v == 0) && !isDJ) {
            evt = EventManager.getInstance().createEvent(EEventType.EVT_IDLE);
            EventManager.getInstance().broadcast(evt);
        }
        h = Input.GetAxis("AHor");
        v = Input.GetAxis("AVer");
        if (isDKEnter && !_enemy.isAtkCD()) {
            evt = EventManager.getInstance().createEvent(EEventType.EVT_ENEMY_ATK);
            EventManager.getInstance().broadcast(evt);
        } else if ((h != 0 || v != 0)) {
            evt = EventManager.getInstance().createEvent(EEventType.EVT_ENEMY_MOVE);
            EventManager.getInstance().broadcast(evt);
        }
        if ((h == 0 && v == 0) && !isDKEnter) {
            evt = EventManager.getInstance().createEvent(EEventType.EVT_ENEMY_IDLE);
            EventManager.getInstance().broadcast(evt);
        }
        
        
    }

    void checkLauncher() {
        if (_timer <= 0.0f) {
            Event evt = EventManager.getInstance().createEvent(EEventType.EVT_LAUNCHER);
            EventManager.getInstance().trigger(evt);
            _timer = 10.0f;
        }
    }

    void checkBuff() {
        Vector3 pos = _player.getPosition();
        GameObject obj = _buff.getNearbyObj(pos);
        if (obj) {
            BuffInfo info = _buff.GetBuffInfo(obj);
            Event evt = EventManager.getInstance().createEvent(EEventType.EVT_USE_BUFF, "obj", obj);
            EventManager.getInstance().broadcast(evt);
            evt = EventManager.getInstance().createEvent(EEventType.EVT_PLAYER_EAT_BUFF, "info", info);
            EventManager.getInstance().broadcast(evt);
        }

        pos = _enemy.getPosition();
        obj = _buff.getNearbyObj(pos);
        if (obj) {
            BuffInfo info = _buff.GetBuffInfo(obj);
            Event evt = EventManager.getInstance().createEvent(EEventType.EVT_USE_BUFF, "obj", obj);
            EventManager.getInstance().broadcast(evt);
            evt = EventManager.getInstance().createEvent(EEventType.EVT_ENEMY_EAT_BUFF, "info", info);
            EventManager.getInstance().broadcast(evt);
        }
    }

    void checkCollider() {
        Vector3 playerPos = _player.getPosition();
        Vector3 enemyPos = _enemy.getPosition();
        Transform playerTrans = _player.getTransform();
        Transform enemyTrans = _enemy.getTransform();
        float playerViewMin = _player.getViewMin();
        float playerViewMax = _player.getViewMax();
        float enemyViewMin = _enemy.getViewMin();
        float enemyViewMax = _enemy.getViewMax();
        float distance = Vector3.Distance(playerPos, enemyPos);
        if (distance > 2.8f) { return; }

        if (_player.getStatus() == 2) {
            Vector3 fromVec = playerTrans.forward;
            Vector3 toVec = enemyPos-playerPos;
            toVec -= Vector3.Project(toVec,Vector3.up);
            float angle = Vector3.Angle(fromVec,toVec);
            Vector3 normal = Vector3.Cross(fromVec,toVec);
            angle *= Mathf.Sign(Vector3.Dot(normal,Vector3.up));
            if (angle < 45.0f && angle > -45.0f && _notifyPlayerHurt == 0.0f) {
                Debug.Log("玩家打中敌人");
                Invoke("notifyEnemyHurt", 0.25f);
            }
        }

        if (_enemy.getStatus() == 2) {
            Vector3 fromVec = enemyTrans.forward;
            Vector3 toVec = playerPos-enemyPos;
            toVec -= Vector3.Project(toVec,Vector3.up);
            float angle = Vector3.Angle(fromVec,toVec);
            Vector3 normal = Vector3.Cross(fromVec,toVec);
            angle *= Mathf.Sign(Vector3.Dot(normal,Vector3.up));
            if (angle < 45.0f && angle > -45.0f && _notifyEnemyHurt == 0.0f) {
                Debug.Log("敌人打中玩家");
                Invoke("notifyPlayerHurt", 0.25f);
            }
        }
    }

    void onEvtBuff(IEvent evt) {
        Vector3 pos = (Vector3)evt.getArg("pos");
        _buff.createBuff(pos);
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

    void notifyPlayerHurt() {
        _notifyPlayerHurt = 0.5f;
        Event evt = EventManager.getInstance().createEvent(EEventType.EVT_PLAYER_HURT, "hurtNum", _enemy.getAtk());
        evt.addArg("rotation", _enemy.getTransform().rotation);
        EventManager.getInstance().broadcast(evt);
        
    }

    void notifyEnemyHurt() {
        _notifyEnemyHurt = 0.5f;
        Event evt = EventManager.getInstance().createEvent(EEventType.EVT_ENEMY_HURT, "hurtNum", _player.getAtk());
        evt.addArg("rotation", _player.getTransform().rotation);
        EventManager.getInstance().broadcast(evt);
    }
}
