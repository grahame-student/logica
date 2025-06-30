namespace CsvOutput;

using LibLogica.Blocks.Width8Bit;

internal class Program
{
    static void Main(string[] args)
    {
        IEnumerable<String> output    = new List<String>();
        output = output.Append(
            "Time [s],Channel 0,Channel 1,Channel 2,Channel 3,Channel 4,Channel 5,Channel 6,Channel 7,Channel 8,Channel 9,Channel 10,Channel 11,Channel 12,Channel 13,Channel 14,Channel 15");

        // Initialise
        var testBlock = new Adder8Bit();
        testBlock.Update();

        UInt32 tick = 0;
        Int32  aVal = 0;
        Int32  bVal = 1;

        while (tick < 1000000)
        {
            List<UInt32> values = testBlock.GetValues().Take(16).Select(b => b ? 1U : 0U).ToList();
            output = output.Append($"{tick / 1000000000.0:f9}," + String.Join(",", values));
            tick++;

            IList<Boolean> aBool = ToBooleanList(aVal, 8);
            IList<Boolean> bBool = ToBooleanList(bVal, 8);
            for (Int32 i = 0; i < aBool.Count(); i++)
            {
                testBlock.A[i].Value = aBool[i];
                testBlock.B[i].Value = bBool[i];
            }
            testBlock.Update();
            aVal += bVal;
        }

        File.WriteAllLines("output.csv", output);
    }

    private static IList<Boolean> ToBooleanList(Int32 value, Int32 width)
    {
        Boolean[] result = new Boolean[width];
        for (Int32 bit = 0; bit < width; bit++)
        {
            Int32 mask = (Int32)Math.Pow(2, bit);
            result[bit] = (value & mask) != 0;
        }

        return result;
    }
}
