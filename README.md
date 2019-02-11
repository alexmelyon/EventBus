# EventBus
Simple system for comminication between different systems.

# Initialize
    EventBus.Instance.Register(this);
    EventBus.Instance.Unregister(this);

# Usage
Handle event
    [OnEvent]
    public void OnEvent(FirstEvent event) {
        //
    }

Send event
    EventBus.Instance.PostEvent(new FirstEvent());
