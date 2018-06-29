using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class EventBus
{
	public static EventBus Instance { get { return instance ?? (instance = new EventBus()); } }

	public void Register(object listener)
	{
		if (!listeners.Any(l => l.Listener == listener))
			listeners.Add(new EventListenerWrapper(listener));
	}

	public void Unregister(object listener)
	{
		listeners.RemoveAll(l => l.Listener == listener);
	}

	public void PostEvent(object e)
	{
//		listeners.Where(l => l.EventType == e.GetType()).ToList().ForEach(l => l.PostEvent(e));
		listeners.Where(l => l.typeToMethod.Keys.Contains(e.GetType()))
			.ToList()
			.ForEach(l => l.PostEvent(e));
	}

	private static EventBus instance;

	private EventBus() { }

	private List<EventListenerWrapper> listeners = new List<EventListenerWrapper>();

	private class EventListenerWrapper
	{
		public object Listener { get; private set; }
		//		public Type EventType { get; private set; }
		public Dictionary<Type, MethodBase> typeToMethod = new Dictionary<Type, MethodBase>();

//		private MethodBase method;
		private MethodBase[] methods;

		public EventListenerWrapper(object listener)
		{
			Listener = listener;

			Type type = listener.GetType();

//			method = type.GetMethod("OnEvent");
//			if (method == null)
//				throw new ArgumentException("Class " + type.Name + " does not containt method OnEvent");
			List<MethodInfo> allMethods = type.GetMethods().ToList();
			for(int i = allMethods.Count - 1; i >= 0; i--) {
				if(allMethods[i].Name != "OnEvent") {
					allMethods.RemoveAt(i);
				}
			}
			methods = allMethods.ToArray();
			if(methods.Length == 0)
				throw new ArgumentException("Class " + type.Name + " does not containt method OnEvent");

			foreach(MethodBase method in methods) {
				ParameterInfo[] parameters = method.GetParameters();
				if (parameters.Length != 1)
					throw new ArgumentException("Method OnEvent of class " + type.Name + " have invalid number of parameters (should be one)");

//				EventType = parameters[0].ParameterType;
				typeToMethod.Add(parameters[0].ParameterType, method);
			}
		}

		public void PostEvent(object e)
		{
//			method.Invoke(Listener, new[] { e });
			MethodBase method = typeToMethod[e.GetType()];
			method.Invoke(Listener, new[] { e });
		}
	}      
}

