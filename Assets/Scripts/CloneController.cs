using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneController : MonoBehaviour {

	/// A test dummy to test core functionality.
	public GameObject dummy;

	/// Denotes whether or not the player is in control.
	[SerializeField] private bool isPlayer;

	/// The starting position of this GameObject.
	private Vector2 startingPosition;
	/// The starting rotation of this GameObject.
	private Quaternion startingRotation;

	public Replayer replayer;

	protected virtual void Awake() {
		this.isPlayer = true;
		GetComponent<SpriteRenderer>().color = Color.red; // debug
		this.startingPosition = transform.position;
		this.startingRotation = transform.rotation;
	} // Awake

	protected virtual void Start() {
		// All the clones need to move together based on when the first clone started to move
		this.replayer = new Replayer(Time.time - CloneManager.instance.offset, this.gameObject);

		replayer.RegisterHandler("Fire1", (sender, eventArgs) => {
			GameObject copy = Instantiate(dummy, eventArgs.mousePosition, Quaternion.identity);
			Destroy(copy, 1);
		});
		replayer.RegisterHandler("HorzVertAxis", (sender, eventArgs) => {
			transform.Translate(new Vector2(eventArgs.horizontalAxis, eventArgs.verticalAxis) * 0.5f);
		});

		CloneManager.instance.AddClone(this);
	} // Start

	protected virtual void Update() {
		if (isPlayer) {
			// Record inputs if this is the player
			if (Input.GetButtonDown("Fire1")) {
				replayer.RecordInput(new InputInTime("Fire1"));
			} // if
		} // if
	} // Update

	protected virtual void FixedUpdate() {
		// Movement code makes too many events in Update
		if (isPlayer) {
			float horizontalAxis = Input.GetAxis("Horizontal");
			float verticalAxis = Input.GetAxis("Vertical");
			if (horizontalAxis != 0 || verticalAxis != 0) {
				replayer.RecordInput(new InputInTime("HorzVertAxis", horizontalAxis, verticalAxis));
			} // if
		} // if
	} // FixedUpdate

	public virtual void ResetToInitialState() {
		// Spawn one and only one additional clone for the player to control
		if (isPlayer) {
			Instantiate(this.gameObject, Vector3.zero, Quaternion.identity);
			GetComponent<SpriteRenderer>().color = Color.white; // debug
		} // if

		isPlayer = false;
		transform.position = startingPosition;
		transform.rotation = startingRotation;
	} // ResetToInitialState

} // CloneController
