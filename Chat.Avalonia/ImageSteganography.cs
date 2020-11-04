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
        
        public string ConcealMessage(string message)
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
            
            string filePath = "Sent/" + Guid.NewGuid() + ".bmp";
            System.IO.File.WriteAllBytes(filePath, MyFile);
            
            return filePath;
        }
        
        public string RevealMessage(Image concealedMessage)
        {
            MyImage = concealedMessage ?? throw new Exception(@"Brak obrazu w otrzymanej wiadomości!");

            ImageToByteArrayConversion();
            var imgBits = new BitArray(MyFile!);
            //Console.WriteLine(BitConverter.ToString(MyFile).Substring(0,500) + "\n----------------------------------");
            int k = 0;
            int begin = ReadData(10, 13);

            BitArray tmp = new BitArray(imgBits.Length / 4 );
            for (int j = begin; j < begin + tmp.Length - 1; j++)
            {
                if ((j * 8) >= imgBits.Length - 1)
                {
                    break;
                }
                tmp[k] = imgBits[j * 8];
                k++;
                tmp[k] = imgBits[(j * 8) + 1];
                k++;
            }
            string decodedMessage = BitsToStringConversion(tmp);
            
            return decodedMessage;
        }

        
        private string BitsToStringConversion(BitArray tmp)
        {
            byte[] decrypted = new byte[tmp.Length / 2];
            tmp.CopyTo(decrypted, 0);
            string decoded = Encoding.UTF8.GetString(decrypted);
            int finIndex = 20;
            if (decoded.Contains("&fi"))
            {
                Console.WriteLine("widzi &fi");
                finIndex = decoded.IndexOf("&fi"); //("&fin", 0, StringComparison.Ordinal);
            }

            
            return decoded.Substring(0, finIndex);
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