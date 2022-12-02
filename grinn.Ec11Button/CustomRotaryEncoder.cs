using Iot.Device.RotaryEncoder;

namespace grinn.Ec11Button;

public class CustomRotaryEncoder : QuadratureRotaryEncoder
{
    private double _lastPulseCount;

    public event EventHandler<RotaryEncoderDirectionArgs>? OnEncoderChange;

    public CustomRotaryEncoder(int pinA, int pinB, int pulsesPerRotation) : base(pinA, pinB, pulsesPerRotation)
    {
        PulseCountChanged += HandlePulseCountChanged;
    }

    private void HandlePulseCountChanged(object? sender, RotaryEncoderEventArgs args)
    {
        // set this so on the next pulse count change we can determine if the pulse count has increased (clockwise turn)
        // or decreased (counter clockwise turn).
        _lastPulseCount = args.Value;
        
        var rotationDirection = args.Value < _lastPulseCount ? 
            RotationDirection.Counterclockwise : RotationDirection.Clockwise;
        
        OnEncoderChange?.Invoke(this, new RotaryEncoderDirectionArgs(rotationDirection));
    }
}

public enum RotationDirection
{
    Clockwise = 0,
    Counterclockwise = 1
}

public class RotaryEncoderDirectionArgs
{
    public RotationDirection Value { get; }

    public RotaryEncoderDirectionArgs(RotationDirection direction)
    {
        Value = direction;
    }
}