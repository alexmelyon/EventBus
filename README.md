# EventBus
Simple system for comminication between different systems.

# Initialize
    EventBus.Instance.Register(this);
    EventBus.Instance.Unregister(this);

# Usage
    public void OnEvent(FirstEvent event) {
        //
    }

    public void OnEvent(SecondEvent event) {
        //
    }
