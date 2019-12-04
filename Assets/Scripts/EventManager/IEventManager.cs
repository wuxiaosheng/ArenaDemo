using System.Collections;
using System.Collections.Generic;
public interface IEventManager {
    bool addEventListener(EEventType gEvt, Handler handler);
    bool removeEventListener(EEventType gEvt);
    bool removeEventListener(EEventType gEvt, Handler handler);
    void trigger(IEvent evt);
    void broadcast(IEvent evt);
    void destroy();
}