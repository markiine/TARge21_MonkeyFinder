namespace MonkeyFinder.ViewModel;
using MonkeyFinder.Services;

public partial class MonkeysViewModel : BaseViewModel
{
    MonkeyService monkeyService;
    public ObservableCollection<Monkey> Monkeys { get; } = new();

    IConnectivity connectivity;

    public MonkeysViewModel(MonkeyService monkeyService, IConnectivity connectivity)
    {
        Title = "Monkey Finder";
        this.monkeyService = monkeyService;
        this.connectivity = connectivity;
    }

    [RelayCommand]
    async Task GoToDetailsAsync(Monkey monkey)
    {
        if (monkey == null)
            return;

        await Shell.Current.GoToAsync($"{nameof(DetailsPage)}", true,
            new Dictionary<string, object>
            {
                {"Monkey", monkey}
            });
    }

    [RelayCommand]
    async Task GetMonkeysAsync()
    {
        if (IsBusy)
            return;

        try
        {
            if (connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Internet issue!",
                    $"Check your internet connection and try again.", "OK");
                return;
            }

            IsBusy = true;
            var monkeys = await monkeyService.GetMonkeys();

            if (Monkeys.Count != 0)
                Monkeys.Clear();

            foreach (var monkey in monkeys)
                Monkeys.Add(monkey);

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get monkeys: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}