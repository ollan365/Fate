using UnityEngine;

public class BackgroundMove : MonoBehaviour
{
	[SerializeField] private Transform[] positions;
	[SerializeField] private Vector3 target;
	[SerializeField] private float scrollAmount;
	[SerializeField] private float moveSpeed;
	private bool isStop;
	public bool IsStop { set => isStop = !isStop; }

	private void Update()
	{
		if (!isStop)
		{
			foreach(Transform t in positions)
				t.position += Vector3.left * moveSpeed * Time.deltaTime;
		}
	}
}

