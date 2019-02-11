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

	public T PostEvent<T>(T e)
	{
		listeners.Where(l => l.typeToMethod.Keys.Contains(e.GetType()))
			.ToList()
			.ForEach(l => l.PostEvent(e));
		return e;
	}

	private static EventBus instance;

	private EventBus() { }

	private List<EventListenerWrapper> listeners = new List<EventListenerWrapper>();

	private class EventListenerWrapper
	{
		public object Listener { get; private set; }
		public Dictionary<Type, List<MethodBase>> typeToMethod = new Dictionary<Type, List<MethodBase>>();

		private MethodBase[] methods;

		public EventListenerWrapper(object listener)
		{
			Listener = listener;

			Type type = listener.GetType();

			List<MethodInfo> allMethods = type.GetMethods().ToList();
			for(int i = allMethods.Count - 1; i >= 0; i--) {
				if(allMethods[i].GetCustomAttributes(typeof(OnEvent), false).Length == 0) {
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

				Type eventType = parameters[0].ParameterType;
				if(!typeToMethod.ContainsKey(eventType)) {
					typeToMethod.Add(eventType, new List<MethodBase>());
				}
				typeToMethod[parameters[0].ParameterType].Add(method);
			}
		}

		public void PostEvent(object e)
		{
			List<MethodBase> methods = typeToMethod[e.GetType()];
			foreach(MethodBase method in methods) {
				method.Invoke(Listener, new[] { e });
			}
		}
	}      
}

