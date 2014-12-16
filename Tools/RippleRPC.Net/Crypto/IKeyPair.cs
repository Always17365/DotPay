using Org.BouncyCastle.Math;

namespace RippleRPC.Net.Crypto
{
    public interface IKeyPair
    {
        string PubHex();

        BigInteger Pub();

        byte[] PubBytes();

        string PrivHex();

        BigInteger Priv();

        bool Verify(byte[] data, byte[] sigBytes);

        byte[] Sign(byte[] bytes);
    }
}
