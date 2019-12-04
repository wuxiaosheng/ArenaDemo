using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCtrl : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerCtrl _player;
    void Start()
    {
        _player = new PlayerCtrl();
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
        bool isDSpace = Input.GetKeyDown(KeyCode.Space);
        if (h != 0 || v != 0) {
            Event evt = EventManager.getInstance().createEvent(EEventType.EVT_MOVE);
            EventManager.getInstance().trigger(evt);
        } else if (isDSpace) {
            Event evt = EventManager.getInstance().createEvent(EEventType.EVT_ATK);
            EventManager.getInstance().trigger(evt);
        }

    }
}
