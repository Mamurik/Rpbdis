using RadiostationWeb.ViewModels;

namespace RadiostationWeb.Services
{
    public interface IBroadcastSheduleService
    {
        HomeViewModel GetHomeViewModel(int numberRows = 10);
    }
}