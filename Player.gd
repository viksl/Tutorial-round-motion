extends CharacterBody3D

@export var currentBody:Node3D
@export var speed:float;
@export var jumpSpeed:float;
@export var gravityScale:float;
@onready var GRAVITY:float = ProjectSettings.get("physics/3d/default_gravity") as float

var translationVelocity:Vector3
var verticalSpeed:float


func _physics_process(delta: float) -> void:
	var inputVector = Input.get_vector("Left", "Right", "Forward", "Backward")
	
	# Translation
	translationVelocity = global_basis * Vector3(inputVector.x, 0, inputVector.y) * speed
	
	# Apply gravity
	if is_on_floor():
		# This is here to make sure there's at least a little bit of gravity to trigger is_on_floor
		verticalSpeed = -gravityScale * GRAVITY * delta
	else:
		verticalSpeed -= gravityScale * GRAVITY * delta

	# Apply jump
	if is_on_floor() and Input.is_action_just_pressed("Jump"):
		verticalSpeed += jumpSpeed
	
	velocity = translationVelocity + verticalSpeed * up_direction
	
	move_and_slide()
	align_with_surface()

	
func align_with_surface() -> void:
	if currentBody == null:
		return
	
	var surfaceNormal = currentBody.global_position.direction_to(global_position)
	var rotatedXform = align_with_normals(global_transform, surfaceNormal)
	global_transform = rotatedXform
	up_direction = surfaceNormal
	
func align_with_normals(xform:Transform3D, normal:Vector3) -> Transform3D:
	xform.basis.y = normal;
	xform.basis.x = -xform.basis.z.cross(normal)
	xform.basis = xform.basis.orthonormalized()
	
	return xform;
	
