namespace CheapFlights.Domain.Entities
{
    public class Passengers
    {
        public Passengers(int adultsCount, int childCount, int infantCount)
        {
            AdultsCount = adultsCount;
            ChildCount = childCount;
            InfantCount = infantCount;
        }

        public int AdultsCount;
        public int ChildCount;
        public int InfantCount;

        public int TotalCount()
        {
            return AdultsCount + ChildCount + InfantCount;
        }
    }
}
