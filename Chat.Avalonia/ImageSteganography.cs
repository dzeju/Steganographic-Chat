using System;
using System.Collections;
using System.Drawing;
using System.Text;

namespace Chat.Avalonia
{
    public class ImageSteganography
    {
        public Image MyImage { get; set; }
        private byte[] MyFile { get; set; }

        public ImageSteganography()
        {
            MyImage = null;
            MyFile = null;
        }
        
        public Image ConcealMessage(string message)
        {
            if (MyImage == null)
            {
                throw new Exception(@"Brak obrazu!");
            }
            ImageToByteArrayConversion();
            
            var msgBits = StringToBitsConversion(message);
            var imgBits = new BitArray(MyFile!);
            
            int k = 0;
            int begin = ReadData(10, 13);
            
            for (int j = begin; j < begin + msgBits.Length - 1; j++)
            {
                imgBits.Set(j * 8, msgBits[k]); 
                k++;
                if(k >= msgBits.Length - 1) break;
                imgBits.Set((j * 8) + 1, msgBits[k]); 
                k++;
            }

            BitsToImageConversion(imgBits);
            
            SaveImage();
            
            return MyImage;
        }

        private void SaveImage()
        {
            System.IO.File.WriteAllBytes("Sent/LastSent.bmp", MyFile);
        }

        private void ImageToByteArrayConversion()
        {
            ImageConverter converter = new ImageConverter();
            MyFile = (byte[]) converter.ConvertTo(MyImage, typeof(byte[]));
        }

        private void BitsToImageConversion(BitArray bits)
        {
            ImageConverter converter = new ImageConverter();
            bits.CopyTo(MyFile!, 0);
            MyImage = (Image) converter.ConvertFrom(MyFile);
        }

        private BitArray StringToBitsConversion(string message)
        {
            byte[] byteStr = Encoding.UTF8.GetBytes(message);
            var messageBits = new BitArray(byteStr);

            return messageBits;
        }
        
        private Int32 ReadData(int begin, int end)
        {
            string data = "";
            for (int i = begin; i < end; i++)
            {
                if (MyFile != null && MyFile[i] != 0)
                {
                    data += MyFile[i].ToString("X");
                }
            }
            return data == "" ? 0 : Convert.ToInt32(data, 16);
        }
    }
}