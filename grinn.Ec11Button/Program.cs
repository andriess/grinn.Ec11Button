// See https://aka.ms/new-console-template for more information

using System.Device.Gpio;
using Iot.Device.RotaryEncoder;

Console.WriteLine("Hello, World!");

const int encoderPinA = 17;
const int encoderPinB = 18;

var rotaryButton = new QuadratureRotaryEncoder(encoderPinA, encoderPinB, PinEventTypes.Falling, 20);
rotaryButton.Debounce = TimeSpan.FromMilliseconds(10);
rotaryButton.PulseCountChanged += OnPulseCountChanged;

await Task.Delay(Timeout.Infinite);

void OnPulseCountChanged(object? sender, RotaryEncoderEventArgs args)
{
    var counter = Math.Abs(args.Value - (-1)) < 0.1 ? "Counter" : string.Empty;
    Console.WriteLine($"{DateTime.Now} - {counter} clockwise ");

}