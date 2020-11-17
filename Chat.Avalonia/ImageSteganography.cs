using System;
using System.Collections;
using System.Collections.Generic;
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
                throw new  Exception(@"Brak obrazu!");
            }
            ImageToByteArrayConversion();
            
            var msgBits = StringToBitsConversion(message);
            var imgBits = new BitArray(MyFile!);
            
            var k = 0;
            var begin = ReadData(10, 13) + 80;

            var counter = new InvertedBitsCounter();
            
            for (var j = begin; j < begin + msgBits.Length - 1; j++)
            {
                bool[] s = {imgBits[j * 8 + 1], imgBits[j * 8 + 2]};
                if (imgBits[j * 8] != msgBits[k])
                {
                    imgBits.Set(j * 8, msgBits[k]);
                    UpdateCount(counter, s, true);
                }
                else
                {
                    UpdateCount(counter, s, false);
                }
                k++;
            }

            imgBits = InvertBits(counter, imgBits, begin, k);
            imgBits = StoreInstructions(counter, imgBits, begin - 80);
            
            BitsToImageConversion(imgBits);
            
            var filePath = "Sent/" + Guid.NewGuid() + ".bmp";
            System.IO.File.WriteAllBytes(filePath, MyFile);
            
            return filePath;
        }
        
        public string RevealMessage(Image concealedMessage)
        {
            MyImage = concealedMessage ?? throw new Exception(@"Brak obrazu w otrzymanej wiadomości!");

            ImageToByteArrayConversion();
            var imgBits = new BitArray(MyFile!);
            var k = 0;
            var begin = ReadData(10, 13) + 80;
            
            var counter = new InvertedBitsCounter();
            ReadInstructions(counter, imgBits, begin - 80);
            imgBits = InvertBits(counter, imgBits, begin, (imgBits.Length - begin) / 8);

            var tmp = new BitArray(imgBits.Length / 4 );
            for (var j = begin; j < begin + tmp.Length - 1; j++)
            {
                if (j * 8 >= imgBits.Length - 1)
                {
                    break;
                }
                tmp[k] = imgBits[j * 8];
                k++;
            }
            var decodedMessage = BitsToStringConversion(tmp);
            
            return decodedMessage;
        }
        
        private static void UpdateCount(InvertedBitsCounter c, IReadOnlyList<bool> bits, bool isChanged)
        {
            switch (bits[0])
            {
                case false when bits[1] == false:
                    if (isChanged) 
                        c.Changed00++;
                    else 
                        c.Changed00--;
                    break;
                case false when bits[1] == true:
                    if (isChanged) 
                        c.Changed01++;
                    else 
                        c.Changed01--;
                    break;
                case true when bits[1] == false:
                    if (isChanged) 
                        c.Changed10++;
                    else 
                        c.Changed10--;
                    break;
                case true when bits[1] == true:
                    if (isChanged) 
                        c.Changed11++;
                    else 
                        c.Changed11--;
                    break;
            }
        }
        
        private static BitArray InvertBits(InvertedBitsCounter c ,BitArray imgBits, int begin, int k)
        {
            for (var j = begin; j < begin + k; j++)
            {
                if (j * 8 + 1 > imgBits.Length) 
                    break;
                bool[] s = {imgBits[j * 8 + 1], imgBits[j * 8 + 2]};
                if ((c.Changed00 <= 0 || s[0] != false || s[1] != false) &&
                    (c.Changed01 <= 0 || s[0] != false || s[1] != true) &&
                    (c.Changed10 <= 0 || s[0] != true || s[1] != false) &&
                    (c.Changed11 <= 0 || s[0] != true || s[1] != true)) continue;
                imgBits.Set(j * 8, !imgBits[j * 8]);
            }
            
            return imgBits;
        }
        
        private static BitArray StoreInstructions(InvertedBitsCounter c, BitArray imgBits, int begin)
        {
            var count = Convert.ToInt32(c.Changed00 > 0) + Convert.ToInt32(c.Changed01 > 0) +
                        Convert.ToInt32(c.Changed10 > 0) + Convert.ToInt32(c.Changed11 > 0);
            if (count > 0)
            {
                imgBits.Set(begin * 8, true);
                imgBits.Set(begin * 8 + 8, c.Changed00 > 0);
                imgBits.Set(begin * 8 + 16, c.Changed01 > 0);
                imgBits.Set(begin * 8 + 24, c.Changed10 > 0);
                imgBits.Set(begin * 8 + 32, c.Changed11 > 0);
            }
            else
            {
                imgBits.Set(begin, false);
            }

            return imgBits;
        }
        
        private static void ReadInstructions(InvertedBitsCounter c, BitArray imgBits, int begin)
        {
            if (!imgBits[begin * 8]) return;
            if (imgBits[begin * 8 + 8] == true)
                c.Changed00 = 1;
            else
                c.Changed00 = -1;
            if (imgBits[begin * 8 + 16] == true)
                c.Changed01 = 1;
            else
                c.Changed01 = -1;
            if (imgBits[begin * 8 + 24] == true)
                c.Changed10 = 1;
            else
                c.Changed10 = -1;
            if (imgBits[begin * 8 + 32] == true)
                c.Changed11 = 1;
            else
                c.Changed11 = -1;
        }

        
        private static string BitsToStringConversion(BitArray tmp)
        {
            var decrypted = new byte[tmp.Length / 2];
            tmp.CopyTo(decrypted, 0);
            var decoded = Encoding.UTF8.GetString(decrypted);
            var finIndex = 20;
            if (decoded.Contains("&fi"))
            {
                finIndex = decoded.IndexOf("&fi", StringComparison.Ordinal);
            }
            
            return decoded.Substring(0, finIndex);
        }

        private void ImageToByteArrayConversion()
        {
            var converter = new ImageConverter();
            MyFile = (byte[]) converter.ConvertTo(MyImage, typeof(byte[]));
        }

        private void BitsToImageConversion(BitArray bits)
        {
            var converter = new ImageConverter();
            bits.CopyTo(MyFile!, 0);
            MyImage = (Image) converter.ConvertFrom(MyFile);
        }

        private static BitArray StringToBitsConversion(string message)
        {
            var byteStr = Encoding.UTF8.GetBytes(message);
            var messageBits = new BitArray(byteStr);

            return messageBits;
        }
        
        private int ReadData(int begin, int end)
        {
            var data = "";
            for (var i = begin; i < end; i++)
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