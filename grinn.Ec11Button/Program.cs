// See https://aka.ms/new-console-template for more information

using grinn.Ec11Button;
using Iot.Device.RotaryEncoder;

Console.WriteLine("Hello, World!");

const int encoderPinA = 17;
const int encoderPinB = 18;

var rotaryButton = new CustomRotaryEncoder(encoderPinA, encoderPinB, 20);
rotaryButton.Debounce = TimeSpan.FromMilliseconds(100);
rotaryButton.PulseCountChanged += HandlePulseCountChange;
rotaryButton.OnEncoderChange += HandleEncoderChange;

await Task.Delay(Timeout.Infinite);

void HandlePulseCountChange(object? sender, RotaryEncoderEventArgs args)
{
    Console.WriteLine($"{nameof(HandlePulseCountChange)} - {args.Value}");
}

void HandleEncoderChange(object? sender, RotaryEncoderDirectionArgs args)
{
    Console.WriteLine($"{nameof(HandleEncoderChange)} - {args.Value}");
}