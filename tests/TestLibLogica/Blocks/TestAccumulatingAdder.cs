namespace TestLibLogica.Blocks;

public class TestAccumulatingAdder
{
    /*
     *         A[8] = 0 <- initial value
     *         A[8] := Q[8]
     *  +-------+
     *  |       |        B [8] = Value to add
     *  |       V        V
     *  |   +---------------+
     *  |   |               |
     *  |   |  8-bit Adder  + < CI = 0
     *  |   |               |
     *  |   +---------------+
     *  |          V
     *  |        D [8]
     *  |          V
     *  |   +---------------+
     *  |   |               |
     *  |   |  8-bit Latch  + < Clk = 1 When Q to be passed to adder
     *  |   |               |
     *  |   +---------------+
     *  |          |
     *  +----------+ Q [8]
     *             |
     *             V
     *           Result
     *     
     *     
     *      QN outputs unused
     *      D -> Q   when   clk == 1
     *
     */
}
