using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	protected static T _instance;

	private static readonly object _lock = new object();

	public static T Instance
	{
		get
		{
			lock (_lock)
			{
				if (_instance == null)
				{
					_instance = (T)FindObjectOfType(typeof(T));

					if (FindObjectsOfType(typeof(T)).Length > 1)
					{
						return _instance;
					}

					if (_instance == null)
					{
						GameObject singleton = new GameObject();
						_instance = singleton.AddComponent<T>();
						singleton.name = "[singleton] " + typeof(T);
						singleton.hideFlags = HideFlags.None;

						DontDestroyOnLoad(singleton);
					}
					else
					{
						Debug.Log("[Singleton] Using instance already created: " + _instance.gameObject.name);
					}
				}

				_instance.hideFlags = HideFlags.None;
				return _instance;
			}
		}
	}

	protected virtual void OnDestroy()
	{
		_instance = null;
	}

	protected virtual void Awake()
	{
	}

	protected virtual void Start()
	{
	}
}