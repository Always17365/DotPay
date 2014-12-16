namespace RippleRPC.Net.Model
{ 
    public struct TransactionType
    {
        public static readonly string Payment = "Payment";
        public static readonly string OfferCreate = "OfferCreate";
        public static readonly string OfferCancel = "OfferCancel";
        public static readonly string TrustSet = "TrustSet"; 
        public static readonly string AccountSet = "AccountSet";
    }

    public enum RippleType
    {
        Int16 = 1,
        Int32 = 2,
        Int64 = 3,
        Hash128 = 4,
        Hash256 = 5,
        Amount = 6,
        VL = 7,
        Account = 8,
        Object = 14,
        Array = 15,
        Int8 = 16,
        Hash160 = 17,
        PathSet = 18,
        Vector256 = 19
    }
}
