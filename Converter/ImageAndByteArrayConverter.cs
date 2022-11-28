using System.Drawing;
using System.IO;

namespace MerchantForm.Converter
{
    public class ImageAndByteArrayConverter : IImageAndByteArrayConverter
    {

        public byte[] ImageToByteArray(Image x)
        {
            ImageConverter _imageConverter = new ImageConverter();
            byte[] xByte = (byte[])_imageConverter.ConvertTo(x, typeof(byte[]));
            return xByte;

        }

        public Image ByteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        /*   public byte[] imageToByteArray(System.Drawing.Image imageIn)
          {
              MemoryStream ms = new MemoryStream();
              imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
              return ms.ToArray();
          }*/


    }
}
