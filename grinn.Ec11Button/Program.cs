// See https://aka.ms/new-console-template for more information

using System.Device.Gpio;

Console.WriteLine("Hello, World!");

const int encoderPinA = 17;
const int encoderPinB = 18;

using var controller = new GpioController();
controller.OpenPin(encoderPinA, PinMode.Input, PinValue.High);
controller.OpenPin(encoderPinB, PinMode.Input, PinValue.High);

controller.RegisterCallbackForPinValueChangedEvent(encoderPinA, 
    PinEventTypes.Falling | PinEventTypes.Rising, OnPinEvent);
controller.RegisterCallbackForPinValueChangedEvent(encoderPinB, 
    PinEventTypes.Falling | PinEventTypes.Rising, OnPinEvent);

await Task.Delay(Timeout.Infinite);

void OnPinEvent(object sender, PinValueChangedEventArgs args)
{
    var otherPin = args.PinNumber == encoderPinA ? encoderPinA : encoderPinB;
    var otherPinValue = controller.Read(otherPin);
    var currentPinValue = controller.Read(args.PinNumber);

    Console.WriteLine($"{DateTime.Now} - Pin: {args.PinNumber}, EventType: {args.ChangeType}, CurrentPinValue: {currentPinValue}, OtherPinValue: {otherPinValue}");
}