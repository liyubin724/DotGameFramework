using Dot.Core.Event;

public class EventManager : Dot.Core.Utill.Singleton<EventManager>
{
    private EventDispatcher dispatcher = null;
    protected override void DoInit()
    {
        base.DoInit();
        dispatcher = new EventDispatcher();
    }
    public void RegisterEvent(int eventID, EventHandler handler) => dispatcher.RegisterEvent(eventID, handler);
    public void UnregisterEvent(int eventID, EventHandler handler) => dispatcher.UnregisterEvent(eventID, handler);
    public void TriggerEvent(int eventID) => dispatcher.TriggerEvent(eventID);
    public void TriggerEvent(int eventID, params object[] datas) => dispatcher.TriggerEvent(eventID, datas);
    protected override void DoDestory()
    {
        dispatcher.DoDispose();
        dispatcher = null;

        base.DoDestory();
    }
}
