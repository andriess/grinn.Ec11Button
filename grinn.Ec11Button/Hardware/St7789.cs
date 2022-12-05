
using System.Device.Gpio;
using System.Device.Spi;
using System.Runtime.InteropServices;
using SkiaSharp;


namespace grinn.Ec11Button.Hardware;

public class St7789
{
    private readonly int _width;
    private readonly int _height;
    private readonly int _dc;
    private readonly int? _backlight;
    private readonly GpioController _gpioController;
    private readonly SpiDevice _spi;

    private byte NOP = 0x00;
    private byte SWRESET = 0x01;
    private byte RDDID = 0x04;
    private byte RDDST = 0x09;
    private byte SLPIN = 0x10;
    private byte SLPOUT = 0x11;
    private byte PTLON = 0x12;
    private byte NORON = 0x13;
    private byte INVOFF = 0x20;
    private byte INVON = 0x21;
    private byte DISPOFF = 0x28;
    private byte DISPON = 0x29;
    private byte CASET = 0x2A;
    private byte RASET = 0x2B;
    private byte RAMWR = 0x2C;
    private byte RAMRD = 0x2E;
    private byte PTLAR = 0x30;
    private byte MADCTL = 0x36;
    private byte COLMOD = 0x3A;
    private byte FRMCTR1 = 0xB1;
    private byte FRMCTR2 = 0xB2;
    private byte FRMCTR3 = 0xB3;
    private byte INVCTR = 0xB4;
    private byte DISSET5 = 0xB6;
    private byte GCTRL = 0xB7;
    private byte GTADJ = 0xB8;
    private byte VCOMS = 0xBB;
    private byte LCMCTRL = 0xC0;
    private byte IDSET = 0xC1;
    private byte VDVVRHEN = 0xC2;
    private byte VRHS = 0xC3;
    private byte VDVS = 0xC4;
    private byte VMCTR1 = 0xC5;
    private byte FRCTRL2 = 0xC6;
    private byte CABCCTRL = 0xC7;
    private byte RDID1 = 0xDA;
    private byte RDID2 = 0xDB;
    private byte RDID3 = 0xDC;
    private byte RDID4 = 0xDD;
    private byte GMCTRP1 = 0xE0;
    private byte GMCTRN1 = 0xE1;
    private byte PWCTR6 = 0xFC;
    
    public St7789(int width, int height, int dc, int? backlight, GpioController gpioController, SpiDevice spi)
    {
        _width = width;
        _height = height;
        _dc = dc;
        _backlight = backlight;
        _gpioController = gpioController;
        _spi = spi;

        gpioController.OpenPin(_dc, PinMode.Output);

        if (backlight != null)
        {
            gpioController.OpenPin(backlight.Value, PinMode.Output);
            gpioController.Write(backlight.Value, PinValue.Low);
            Task.Delay(TimeSpan.FromMilliseconds(100));
            gpioController.Write(backlight.Value, PinValue.High);
        }
        
        Init();
    }
    
    public void SendCommand(byte[] data)
    {
        Send(data, false);
    }
    
    public void SendCommand(byte data)
    {
        SendCommand(new [] {data});
    }
    
    public void SendData(byte data)
    {
        SendData(new [] {data});
    }

    public void SendData(int data)
    {
        var intBytes = BitConverter.GetBytes(data);
        
        SendData(intBytes);
    }
    
    public void SendData(byte[] data)
    {
        Send(data, true);
    }
    
    private void Send(byte[] data, bool isData, int chunkSize = 4096)
    {
        _gpioController.Write(_dc, isData);
        
        // The python lib does some scalar to list thing here. 
        // # Convert scalar argument to list so either can be passed as parameter.
        // if isinstance(data, numbers.Number):
        //     data = [data & 0xFF]

        foreach (var dataChunk in data.Chunk(chunkSize))
        {
            _spi.Write(dataChunk);
        }
    }

    private void SetWindows(int x0 = 0, int y0 = 0, int x1 = 0, int y1 = 0)
    {
        if (x1 == 0)
        {
            x1 = _width - 1;
        }

        if (y1 == 0)
        {
            y1 = _height - 1;
        }
        
        SendCommand(CASET);
        SendData(x0 >> 8);
        SendData(x0 & 0xFF);
        SendData(x1 >> 8);
        SendData(x1 & 0xFF);  
        
        SendCommand(RASET);
        SendData(y0 >> 8);
        SendData(y0 & 0xFF);
        SendData(y1 >> 8);
        SendData(y1 & 0xFF);
        SendCommand(RAMWR);
    }

    public void Display(SKImage img)
    {
        SetWindows();

        var bytes = GetBytesForImage(img);

        if (bytes.Length == 0)
            return;
        
        SendData(bytes);
    }

    private static byte[] GetBytesForImage(SKImage img)
    {
        var imageInfo = new SKImageInfo(img.Width, img.Height,  SKColorType.Rgb565, img.AlphaType);
        var bitmap = new SKBitmap(imageInfo);
        if (img.ReadPixels(imageInfo, bitmap.GetPixels(), imageInfo.RowBytes, 0, 0))
        {
            IntPtr ptr = bitmap.GetPixels();
            var rgbBytes = new byte[bitmap.ByteCount];
            Marshal.Copy(ptr, rgbBytes, 0, bitmap.ByteCount);
            
            Console.Write(rgbBytes);
            
            return rgbBytes;
        }
        
        bitmap.Dispose();
        bitmap = null;

        return Array.Empty<byte>();
    }

    private void Init()
    {
        SendCommand(SWRESET);
        Task.Delay(TimeSpan.FromMilliseconds(150));
        
        SendCommand(MADCTL); // Software reset
        SendData(0x70);
        
        SendCommand(FRMCTR2); // Frame rate ctrl - idle mode
        SendData(0x0C);
        SendData(0x0C);
        SendData(0x00);
        SendData(0x33);
        SendData(0x33);
        
        SendCommand(COLMOD);
        SendData(0x05);
        
        SendCommand(GCTRL);
        SendData(0x14);
        
        SendCommand(VCOMS);
        SendData(0x37);
        
        SendCommand(LCMCTRL); // Power control
        SendData(0x2C);
        SendCommand(VDVVRHEN); // Power control
        SendData(0x01);
        SendCommand(VRHS); // Power control
        SendData(0x12);
        SendCommand(VDVS); // Power control
        SendData(0x20);
        
        SendCommand(0xD0);
        SendData(0xA4);
        SendData(0xA1);
        
        SendCommand(FRCTRL2);
        SendData(0x0F);
        
        SendCommand(GMCTRP1); // Set Gamma
        SendData(0xD0);
        SendData(0x04);
        SendData(0x0D);
        SendData(0x11);
        SendData(0x13);
        SendData(0x2B);
        SendData(0x3F);
        SendData(0x54);
        SendData(0x4C);
        SendData(0x18);
        SendData(0x0D);
        SendData(0x0B);
        SendData(0x1F);
        SendData(0x23);
        
        SendCommand(GMCTRN1); // Set Gamma
        SendData(0xD0);
        SendData(0x04);
        SendData(0x0C);
        SendData(0x11);
        SendData(0x13);
        SendData(0x2C);
        SendData(0x3F);
        SendData(0x44);
        SendData(0x51);
        SendData(0x2F);
        SendData(0x1F);
        SendData(0x1F);
        SendData(0x20);
        SendData(0x23);
        
        SendCommand(DISPON);
        Task.Delay(TimeSpan.FromMilliseconds(100));
    }
}