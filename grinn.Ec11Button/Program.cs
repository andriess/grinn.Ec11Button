﻿// See https://aka.ms/new-console-template for more information

using System.Device.Gpio;
using System.Device.Spi;
using grinn.Ec11Button;
using grinn.Ec11Button.Hardware;
using Iot.Device.Spi;
using SkiaSharp;

Console.WriteLine("Hello, World!");

const int encoderPinA = 17;
const int encoderPinB = 18;
const int encoderPinC = 27;

const int displayPinCS = 7;
const int displayPinSCK = 11;
const int displayPinMOSI = 10;
const int displayPinDC = 9;
const int displayPinBL = 19;

var gpioController = new GpioController();

// My single button code. This should move somewhere smarter once we start orchestrating our gui+controls
var rotaryButton = new CustomRotaryEncoder(encoderPinA, encoderPinB, encoderPinC, 20, gpioController);

// Init MPD connection, this is going to fail fast if it can't connect. Which is fine with me. 
var mpdConnection = new MpdSocketConnection("/var/run/mpd/socket");
await mpdConnection.Connect();

// Display tests
var spiConfig = new SpiConnectionSettings(0, 0);
spiConfig.ClockFrequency = 10_000_000; // 10mhz

var d = SpiDevice.Create(spiConfig);

// write reset command
gpioController.Write(displayPinDC, PinValue.Low);;
d.WriteByte(0x01);


/*
 *         SendCommand(SWRESET); // Software reset
        Task.Delay(TimeSpan.FromMilliseconds(150));
        
        SendCommand(MADCTL); 
        SendData((byte)0x70);
 */

/*var spiDevice = new SoftwareSpi(displayPinSCK, -1, displayPinMOSI, displayPinCS, spiConfig, gpioController);
var display = new St7789(240, 240, displayPinDC, displayPinBL, gpioController, d);

using var bitmap = SKBitmap.Decode(@"cat.jpg");
var image = SKImage.FromBitmap(bitmap);

display.SetWindows();*/
//display.Display(image);

await Task.Delay(int.MaxValue);