using CheapFlights.Model;
using CheapFlights.DataTransferObjects;
using Microsoft.AspNetCore.Components.Forms;

namespace CheapFlights.Services
{
    public class PresentationService
    {
        private readonly FlightSearchManager _flightSearchManager;
        public string ErrorMessage { get; private set; }
        public bool IsLoading { get; private set; }
        public bool IsPassengerDropdownOpen { get; private set; }
        public bool IsFlightTypeDropdownOpen { get; private set; }
        public SearchParameters SearchParams { get; } = new SearchParameters();
        public IEnumerable<FlightOfferDto> Results { get; private set; }
        public string ValidationMessage { get; private set; }
        public bool ShowValidationAlert { get; private set; }

        public event Action StateChanged;

        public PresentationService(FlightSearchManager flightSearchManager)
        {
            _flightSearchManager = flightSearchManager;
        }

        private void NotifyStateChanged() => StateChanged?.Invoke();

        public async Task HandleSearchError(string message)
        {
            ValidationMessage = message;
            ShowValidationAlert = true;
            NotifyStateChanged();
        }

        public void HandleInvalidSubmit(EditContext editContext)
        {
            ValidationMessage = string.Join("\n", editContext.GetValidationMessages());
            ShowValidationAlert = true;
            NotifyStateChanged();
        }

        public async Task Search()
        {
            try
            {
                IsLoading = true;
                Results = null;
                ShowValidationAlert = false;
                NotifyStateChanged();

                Results = await _flightSearchManager.Search(SearchParams);
            }
            catch (Exception ex)
            {
                await HandleSearchError(ex.Message);
            }
            finally
            {
                IsLoading = false;
                NotifyStateChanged();
            }
        }

        public void TogglePassengerDropdown()
        {
            IsPassengerDropdownOpen = !IsPassengerDropdownOpen;
            NotifyStateChanged();
        }

        public void ToggleFlightTypeDropdown()
        {
            IsFlightTypeDropdownOpen = !IsFlightTypeDropdownOpen;
            NotifyStateChanged();
        }

        public void SelectFlightType(bool isOneWay)
        {
            SearchParams.IsOneWay = isOneWay;
            IsFlightTypeDropdownOpen = false;
            Results = null;
            ErrorMessage = null;
            NotifyStateChanged();
        }

        public void ResetValidation()
        {
            ShowValidationAlert = false;
            ValidationMessage = null;
            NotifyStateChanged();
        }

        public void ResetSearch()
        {
            Results = null;
            ErrorMessage = null;
            NotifyStateChanged();
        }
    }
}
