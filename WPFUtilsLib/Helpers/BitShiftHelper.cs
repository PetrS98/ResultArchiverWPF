namespace StationTireInspection.Classes.Helper
{
    public static class BitShiftHelper
    {
        public static byte SetBitInByte(ref byte Variable, byte BitPosition, bool Value)
        {
            byte Mask = 0;
            if (Value) Mask = 1;

            Mask = (byte)(Mask << BitPosition);

            if (Value) return (byte)(Variable | Mask);
            return (byte)(Variable & Mask);
        }

        public static bool GetBitFromByte(byte Variable, byte BitPosition)
        {
            byte Mask = 1;

            Mask = (byte)(Mask << BitPosition);

            return (Mask & Variable) > 0;
        }
    }
}
