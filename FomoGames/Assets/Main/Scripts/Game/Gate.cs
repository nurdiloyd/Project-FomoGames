namespace Main.Scripts.Game
{
    public class Gate
    {
        public readonly int ID;
        public readonly int PivotI;
        public readonly int PivotJ;
        public readonly BlockColor GateColor;
        public readonly BlockDirection GateDirection;
        
        public Gate(int id, BlockDirection gateDirection, BlockColor gateColor, int pivotI, int pivotJ)
        {
            ID = id;
            GateDirection = gateDirection;
            GateColor = gateColor;
            PivotI = pivotI;
            PivotJ = pivotJ;
        }
        
        public Gate(Gate refGate)
        {
            ID = refGate.ID;
            GateDirection = refGate.GateDirection;
            GateColor = refGate.GateColor;
            PivotI = refGate.PivotI;
            PivotJ = refGate.PivotJ;
        }
    }
}
