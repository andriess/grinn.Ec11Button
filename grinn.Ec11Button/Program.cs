// See https://aka.ms/new-console-template for more information

using grinn.Ec11Button;

Console.WriteLine("Hello, World!");

const int encoderPinA = 17;
const int encoderPinB = 18;

var rotaryButton = new CustomRotaryEncoder(encoderPinA, encoderPinB, 20);
rotaryButton.OnEncoderChange += HandleEncoderChange;

await Task.Delay(Timeout.Infinite);

void HandleEncoderChange(object? sender, RotaryEncoderDirectionArgs args)
{
    var counter = args.Value.Equals(RotationDirection.Counterclockwise) ? "Counter" : string.Empty;
    Console.WriteLine($"{DateTime.Now} - {counter} clockwise ");
}