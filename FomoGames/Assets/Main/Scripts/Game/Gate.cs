namespace Main.Scripts.Game
{
    public class Gate
    {
        public readonly int ID;
        public readonly int PivotI;
        public readonly int PivotJ;
        public BlockColor GateColor { get; private set; }
        public BlockDirection GateDirection { get; private set; }
        
        public Gate(int id, BlockDirection gateDirection, BlockColor gateColor, int pivotI, int pivotJ)
        {
            ID = id;
            GateDirection = gateDirection;
            GateColor = gateColor;
            PivotI = pivotI;
            PivotJ = pivotJ;
        }
    }
}
