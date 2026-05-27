using CommunityToolkit.Mvvm.ComponentModel;
using IpCalculatorLinux.Models;

namespace IpCalculatorLinux.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _ipAddressInput = "192.168.1.1";

    [ObservableProperty]
    private string _cidrInput = "24";

    [ObservableProperty]
    private NetworkInfo? _result;

    [ObservableProperty]
    private string _errorMessage = "";

    public MainWindowViewModel()
    {
        Calculate();
    }

    partial void OnIpAddressInputChanged(string value)
    {
        Calculate();
    }

    partial void OnCidrInputChanged(string value)
    {
        Calculate();
    }

    private void Calculate()
    {
        ErrorMessage = "";
        
        if (string.IsNullOrWhiteSpace(IpAddressInput) || string.IsNullOrWhiteSpace(CidrInput))
        {
            Result = null;
            return;
        }

        if (!int.TryParse(CidrInput, out int cidr) || cidr < 0 || cidr > 32)
        {
            ErrorMessage = "Nieprawidłowa maska CIDR (wymagane 0-32).";
            Result = null;
            return;
        }

        var res = IpCalculatorLogic.Calculate(IpAddressInput.Trim(), cidr);
        if (res == null)
        {
            ErrorMessage = "Nieprawidłowy adres IP.";
            Result = null;
        }
        else
        {
            Result = res;
        }
    }
}
