using CheapFlights.Domain.Entities;

namespace CheapFlights.Domain.Extensions
{
    public static class PassengersExtensions
    {
        public static void UpdatePassengerCount(this Passengers passengers, PassengerType type, bool increment)
        {
            switch (type)
            {
                case PassengerType.Adult:
                    passengers.AdultsCount = increment
                        ? Math.Min(passengers.AdultsCount + 1, 9)
                        : Math.Max(passengers.AdultsCount - 1, 1);
                    break;
                case PassengerType.Child:
                    passengers.ChildCount = increment
                        ? Math.Min(passengers.ChildCount + 1, 9)
                        : Math.Max(passengers.ChildCount - 1, 0);
                    break;
                case PassengerType.Infant:
                    passengers.InfantCount = increment
                        ? Math.Min(passengers.InfantCount + 1, 9)
                        : Math.Max(passengers.InfantCount - 1, 0);
                    break;
            }
        }
    }

    public enum PassengerType
    {
        Adult,
        Child,
        Infant
    }
}
