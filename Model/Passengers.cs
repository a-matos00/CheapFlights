namespace CheapFlights.Model
{
    public class Passengers
    {
        public Passengers(uint adultsCount, uint childCount, uint infantCount)
        {
            AdultsCount = adultsCount;
            ChildCount = childCount;
            InfantCount = infantCount;
        }

        public uint AdultsCount;
        public uint ChildCount;
        public uint InfantCount;

        public uint TotalCount()
        {
            return AdultsCount + ChildCount + InfantCount;
        }
    }
}
