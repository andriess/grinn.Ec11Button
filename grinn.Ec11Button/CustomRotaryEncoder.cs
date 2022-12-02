using System.Device.Gpio;
using System.Diagnostics;
using Iot.Device.RotaryEncoder;

namespace grinn.Ec11Button;

public class CustomRotaryEncoder : QuadratureRotaryEncoder
{
    private double _lastPulseCount;
    private readonly int _pinC;
    private readonly Stopwatch _debouncer = new ();

    private GpioController _controller;
    public event EventHandler<RotaryEncoderDirectionArgs>? OnEncoderChange;
    
    public event EventHandler<bool>? OnClick;

    public CustomRotaryEncoder(int pinA, int pinB, int pinC, int pulsesPerRotation, GpioController controller)
        : base(pinA, pinB,PinEventTypes.Falling, pulsesPerRotation, controller)
    {
        _controller = controller;
        _pinC = pinC;
        controller.OpenPin(_pinC, PinMode.Input);
        controller.RegisterCallbackForPinValueChangedEvent(_pinC, PinEventTypes.Falling, HandleClickEvent);
    }
    
    public CustomRotaryEncoder(int pinA, int pinB, int pinC, int pulsesPerRotation) 
        : this(pinA, pinB, pinC, pulsesPerRotation, new GpioController())
    {
        
    }

    protected override void OnPulse(bool blnUp, int milliSecondsSinceLastPulse)
    {
        base.OnPulse(blnUp, milliSecondsSinceLastPulse);
        
        var rotationDirection = PulseCount < _lastPulseCount ? 
            RotationDirection.Counterclockwise : RotationDirection.Clockwise;
        
        // set this so on the next pulse count change we can determine if the pulse count has increased (clockwise turn)
        // or decreased (counter clockwise turn).
        _lastPulseCount = PulseCount;
        
        OnEncoderChange?.Invoke(this, new RotaryEncoderDirectionArgs(rotationDirection));
    }
    
    private void HandleClickEvent(object sender, PinValueChangedEventArgs args)
    {
        if ((uint)Debounce.TotalMilliseconds == 0 | _debouncer.ElapsedMilliseconds > (uint)Debounce.TotalMilliseconds)
        {
            OnClick?.Invoke(this, true); 
        }
        
        _debouncer.Restart();
    }

    public new void Dispose()
    {
        _controller.ClosePin(_pinC);
        
        base.Dispose();
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