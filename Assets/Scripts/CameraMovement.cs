using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Player Variables")]
    public bool isPlayerStill; //bool to see if the player is moving, for if we want to enact the idea of 'looking' around further when player is still
    [SerializeField] GameObject player; //connect script on camera towards the player game object to reference it more specifically
    [SerializeField] GameObject mainCamera; //connect script to direct reference of camera object

    [Header("Speed/Follow Variables")]
    private float followSpeed; //speed of which camera moves for if we use damp function
    private float damp; //damp is to soften the speed so it looks smooth, for if we use damp function
    private Vector3 vel = Vector3.zero; //vector that holds the speed information, vel is short for velocity, for if we use damp function
    public Vector3 offset; //vector that is the offset for the player's location and the camera's location


    private void LateUpdate() 
    {
        //this is where we set offset to create the slow dampening area if we wanted there to be delay in camera following player
        //...

        Vector3 targetPosition = player.transform.position + offset; //create vector for recording the location of the target (player) and skews it with the offset
        targetPosition.z = mainCamera.transform.position.z; //ensure the z stays the same so the camera still sees everything

        mainCamera.transform.position = targetPosition; //camera position is updated to the player's position

    }
}
