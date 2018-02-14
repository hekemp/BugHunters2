public class NetworkPlayerTransformWriter
{
    ByteWriter writer = new ByteWriter();
    ByteReader reader = new ByteReader();

    public byte[] GetBytes(PlayerTransformData data)
    {
        writer.Clear();
        writer.Write(data.Position);
        writer.Write(data.HeadPosition);
        writer.Write(data.LeftHandPosition);
        writer.Write(data.RightHandPosition);
        writer.WriteCompressed(data.HeadRotation);
        writer.WriteCompressed(data.LeftHandRotation);
        writer.WriteCompressed(data.RightHandRotation);
        return writer.ToArray();
    }

    public PlayerTransformData FromBytes(byte[] bytes)
    {
        PlayerTransformData data = new PlayerTransformData();

        reader.Replace(bytes);

        data.Position = reader.ReadVector3();
        data.HeadPosition = reader.ReadVector3();
        data.LeftHandPosition = reader.ReadVector3();
        data.RightHandPosition = reader.ReadVector3();
        data.HeadRotation = reader.ReadCompressedRotation();
        data.LeftHandRotation = reader.ReadCompressedRotation();
        data.RightHandRotation = reader.ReadCompressedRotation();

        return data;
    }
}