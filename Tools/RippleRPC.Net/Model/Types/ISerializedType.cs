namespace RippleRPC.Net.Model.Types
{
    interface ISerializedType
    {
        byte[] Serialize(object val);
        object Parse();
        int Id { get; set; }
    }
}
