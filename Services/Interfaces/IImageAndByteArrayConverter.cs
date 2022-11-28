using System.Drawing;

namespace MerchantForm.Converter
{
    public interface IImageAndByteArrayConverter
    {
        Image ByteArrayToImage(byte[] byteArrayIn);
        byte[] ImageToByteArray(Image x);
    }
}