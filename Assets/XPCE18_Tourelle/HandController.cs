using UnityEngine;
using UnityEditor;
using Valve.VR.InteractionSystem;
using Valve.VR;
using DG.Tweening;

[RequireComponent(typeof(SteamVR_Behaviour_Pose))]

public class HandController : MonoBehaviour
{
	[SerializeField] bool showForward = true;
	[SerializeField] float forwardDistance;
	[SerializeField] Vector3 customOffset;

	[SerializeField] float offsetTweenDuration = 0.5f;

	private void Start()
	{
		GetComponent<SteamVR_Behaviour_Pose>().onTransformChangedEvent += AddHandOffset;
	}

	private void AddHandOffset(SteamVR_Behaviour_Pose behavior, SteamVR_Input_Sources fromSource)
	{
		behavior.transform.position += showForward ? behavior.transform.forward * forwardDistance : customOffset;
	}

	public void SetShowForward(bool b)
	{
		showForward = b;
	}

	public void SetForwardDistance(float distance, bool smoothChange = true)
	{
		if (smoothChange)
			DOTween.To(x => forwardDistance = x, 0, distance, offsetTweenDuration);
		else
			forwardDistance = distance;
	}

	public void ChangeOffset(Vector3 offset, bool smoothChange = true)
	{
		if (smoothChange)
			DOTween.To(() => customOffset, x => customOffset = x, offset, offsetTweenDuration);
		else
			customOffset = offset;
	}
}