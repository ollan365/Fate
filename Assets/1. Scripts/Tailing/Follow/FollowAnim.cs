using UnityEngine;

public class FollowAnim : MonoBehaviour
{
	[Header("배경")]
	[SerializeField] private Transform[] positions;
	[SerializeField] private float moveSpeed;

	[Header("캐릭터")]
	[SerializeField] private Player player;
    [SerializeField] private Animator fateBoy, fateGirl, accidyBoy, accidyGirl;
	private Animator fate, accidy;

	private bool isStop;
	public bool IsStop
	{
		set
		{
			isStop = !isStop;
			fate.SetBool("Walking", !isStop);
			accidy.SetBool("Walking", !isStop);
		}
	}
    private void Start()
    {
		isStop = true;
		SetCharcter(1);
    }
    private void Update()
	{
		if (!isStop)
		{
			foreach (Transform t in positions)
				t.position += Vector3.left * moveSpeed * Time.deltaTime;
		}
	}

	public void SetCharcter(int accidyGender)
    {
		if (player.Gender == 0) fate = fateBoy;
		else fate = fateGirl;

		if (accidyGender == 0) accidy = accidyBoy;
		else accidy = accidyGirl;

		fate.gameObject.SetActive(true);
		accidy.gameObject.SetActive(true);
	}
}
