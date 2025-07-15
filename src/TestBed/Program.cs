using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;

using FFMediaToolkit;
using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;

using LibLogica.Blocks.Width8Bit;

namespace TestBed;

internal class Program
{
    static void Main(String[] _)
    {
        var testBlock = new Adder8Bit();
        String headers = String.Join(",", testBlock.GetIds());
        IEnumerable<String> results = new List<String>();

        // Initialise
        testBlock.Update();
        results = results.Append(String.Join(",", testBlock.GetValues()));

        Int32 aVal = 0;
        Int32 bVal = 1;

        while (aVal < 256)
        {
            IList<Boolean> aBool = ToBooleanList(aVal, 8);
            IList<Boolean> bBool = ToBooleanList(bVal, 8);
            for (Int32 i = 0; i < aBool.Count(); i++)
            {
                testBlock.A[i].Value = aBool[i];
                testBlock.B[i].Value = bBool[i];
            }

            testBlock.Update();
            results = results.Append(String.Join(",", testBlock.GetValues()));

            aVal += bVal;
        }

        const Int32 bitSize = 8;
        WriteReport(headers, results);
        WriteImages(results, 32, 16, bitSize);
        WriteVideo(32, 16, bitSize);
    }

    private static IList<Boolean> ToBooleanList(Int32 value, Int32 width)
    {
        Boolean[] result = new Boolean[width];
        for (Int32 bit = 0; bit < width; bit++)
        {
            Int32 mask = (Int32)Math.Pow(2, bit);
            result[bit] = (value & mask) != 0;
        }

        return result;
    }

    private static void WriteReport(String headers, IEnumerable<String> results)
    {
        using var outputFile = new StreamWriter("Results.csv");
        outputFile.WriteLine(headers);

        Console.WriteLine($"Total columns in results - {headers.Split(",").Length}");

        foreach (String result in results)
        {
            outputFile.WriteLine(result);
        }
    }

    private static void WriteImages(IEnumerable<String> results, Int32 width, Int32 height, Int32 bitSize)
    {
        Int32 i = 0;
        foreach (String result in results)
        {
            String[] stringList = result.Split(",");
            IEnumerable<Boolean> boolList = stringList.Select(Boolean.Parse);
            WriteSingleImage(boolList, width, height, i, bitSize);
            i++;
        }
    }

    private static void WriteSingleImage(IEnumerable<Boolean> boolList, Int32 width, Int32 height, Int32 imageNumber, Int32 bitSize)
    {
        Int32 px = 0;
        Int32 py = 0;
        using var b = new Bitmap(width * bitSize, height * bitSize);
        using var g = Graphics.FromImage(b);
        Brush setBrush = Brushes.Black;
        Brush clearBrush = Brushes.White;

        foreach (Boolean point in boolList)
        {
            Brush brush = point ? setBrush : clearBrush;
            g.FillRectangle(brush, px * bitSize, py * bitSize, bitSize, bitSize);
            px++;
            if (px < width) continue;
            px = 0;
            py++;
        }
        b.Save($"image_{imageNumber:000}.png", ImageFormat.Png);
    }

    private static void WriteVideo(Int32 width, Int32 height, Int32 bitSize)
    {
        FFmpegLoader.FFmpegPath = @"C:\ffmpeg-6.1.1\bin\";

        var settings = new VideoEncoderSettings(width * bitSize, height * bitSize, 5, VideoCodec.H265)
        {
            EncoderPreset = EncoderPreset.VerySlow,
            CRF = 0,
        };

        Console.WriteLine($"{Environment.Is64BitProcess}");

        MediaOutput? file = MediaBuilder.CreateContainer(@"C:\temp\video.mp4").WithVideo(settings).Create();
        String[] files = Directory.GetFiles(".", "*.png");

        foreach (String inputFile in files)
        {
            Byte[] binInputFile = File.ReadAllBytes(inputFile);
            var memInput = new MemoryStream(binInputFile);
            var bitmap = Bitmap.FromStream(memInput) as Bitmap;
            var rect = new Rectangle(Point.Empty, bitmap.Size);
            BitmapData bitLock = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            var bitmapData = ImageData.FromPointer(bitLock.Scan0, ImagePixelFormat.Bgr24, bitmap.Size);
            file.Video.AddFrame(bitmapData);
            bitmap.UnlockBits(bitLock);
        }

        file.Dispose();
    }
}
