using Godot;

namespace TutorialRoundMotion;

public partial class Player : CharacterBody3D
{
    // Don't forget to build the project first so you can assign the exported fields in the editor!
    [Export] private Node3D _currentBody;
    [Export] private float _speed;
    [Export] private float _jumpSpeed;
    [Export] private float _gravityScale;

    private float _gravity;
    private Vector3 _translationVelocity;
    private float _verticalSpeed;

    public override void _Ready()
    {
        base._Ready();

        _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
    }
    
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
        var fDelta = (float)delta;
        
        // Translation
        var inputVector = Input.GetVector("Left", "Right", "Forward", "Backward");
        _translationVelocity = GlobalBasis * new Vector3(inputVector.X, 0, inputVector.Y) * _speed;

        // Gravity
        if (IsOnFloor())
        {
            // This is here to make sure there's at least little bit of gravity to trigger IsOnFloor()
            _verticalSpeed = -_gravityScale * _gravity * fDelta;
        }
        else
        {
            _verticalSpeed -= _gravityScale * _gravity * fDelta;
        }
        
        // Apply jump
        if (IsOnFloor() && Input.IsActionJustPressed("Jump"))
        {
            _verticalSpeed += _jumpSpeed;
        }

        Velocity = _translationVelocity + _verticalSpeed * UpDirection;

        MoveAndSlide();
        AlignWithSurface();
    }

    private Transform3D AlignWithNormal(Transform3D xform, Vector3 normal)
    {
        xform.Basis.Y = normal;
        xform.Basis.X = -xform.Basis.Z.Cross(normal);
        xform.Basis = xform.Basis.Orthonormalized();

        return xform;
    }

    private void AlignWithSurface()
    {
        if (_currentBody is null)
            return;

        var surfaceNormal = _currentBody.GlobalPosition.DirectionTo(GlobalPosition);
        var rotatedXform = AlignWithNormal(GlobalTransform, surfaceNormal);
        GlobalTransform = rotatedXform;
        UpDirection = surfaceNormal;
    }
}