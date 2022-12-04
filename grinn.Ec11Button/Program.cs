﻿// See https://aka.ms/new-console-template for more information

using grinn.Ec11Button;

Console.WriteLine("Hello, World!");

const int encoderPinA = 17;
const int encoderPinB = 18;
const int encoderPinC = 27;
// My single button code. This should move somewhere smarter once we start orchestrating our gui+controls
var rotaryButton = new CustomRotaryEncoder(encoderPinA, encoderPinB, encoderPinC, 20);
rotaryButton.Debounce = TimeSpan.FromMilliseconds(175);
rotaryButton.OnEncoderChange += HandleEncoderChange;
rotaryButton.OnClick += HandleClick;


// Trying some MPD stuff here: 
var mpdConnection = new MpdSocketConnection("/var/run/mpd/socket");
var result = await mpdConnection.SendCommandToSocket("ping\n");

Console.WriteLine(result);

var playlistInfo = await mpdConnection.SendCommandToSocket("playlistinfo\n");
Console.WriteLine(playlistInfo);

void HandleClick(object? sender, bool args)
{
    Console.WriteLine($"{nameof(HandleClick)} - click event arrived in main program.");
}

void HandleEncoderChange(object? sender, RotaryEncoderDirectionArgs args)
{
    Console.WriteLine($"{nameof(HandleEncoderChange)} - {args.Value}");
}