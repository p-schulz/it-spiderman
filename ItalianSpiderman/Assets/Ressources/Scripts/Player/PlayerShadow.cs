using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Projector))]
public class PlayerShadow : MonoBehaviour {

    public PlayerMachine Player;
    public Transform CenterOfGravity;

    private Transform target;
    private SuperCharacterController controller;
    private Projector projector;

    void Start()
    {
        target = Player.transform;
        controller = Player.GetComponent<SuperCharacterController>();
        projector = GetComponent<Projector>();
    }

	void LateUpdate () {
        transform.rotation = target.rotation * Quaternion.AngleAxis(90, target.right);

        Transform t = target;

        if (Player.StateCompare(PlayerMachine.PlayerStates.Climb))
        {
            t = CenterOfGravity;
        }

        transform.position = t.position + (controller.up * controller.radius);

        projector.farClipPlane = controller.currentGround.Hit.distance + controller.radius * 6;

	}
}
