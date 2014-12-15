using Connectivity.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Connectivity.Plugin
{
  /// <summary>
  /// Implementation for Connectivity
  /// </summary>
  public class ConnectivityImplementation : IConnectivity
  {

    public ConnectivityImplementation()
    {
      Bandwidths = new List<UInt64>();
      UpdateConnected();
      Reachability.ReachabilityChanged += (sender, args) => UpdateConnected();
    }



    private void UpdateConnected()
    {
      var remoteHostStatus = Reachability.RemoteHostStatus();
      var internetStatus = Reachability.InternetConnectionStatus();
      var localWifiStatus = Reachability.LocalWifiConnectionStatus();
      IsConnected = (internetStatus == NetworkStatus.ReachableViaCarrierDataNetwork ||
                      internetStatus == NetworkStatus.ReachableViaWiFiNetwork) ||
                    (localWifiStatus == NetworkStatus.ReachableViaCarrierDataNetwork ||
                      localWifiStatus == NetworkStatus.ReachableViaWiFiNetwork) ||
                    (remoteHostStatus == NetworkStatus.ReachableViaCarrierDataNetwork ||
                      remoteHostStatus == NetworkStatus.ReachableViaWiFiNetwork);
    }


    public bool IsConnected { get; private set; }

    public Task<bool> IsReachable(string host, int msTimeout = 5000)
    {
      return new Task<bool>(() => Reachability.IsHostReachable(host));
    }

    public Task<bool> IsPortReachable(string host, int port = 80, int msTimeout = 5000)
    {
      return new Task<bool>(() => Reachability.IsHostReachable(host, port));
    }

    public IEnumerable<ConnectionType> ConnectionTypes 
    {
      get
      {
        var status = Reachability.InternetConnectionStatus();
        switch (status)
        {
          case NetworkStatus.ReachableViaCarrierDataNetwork:
            yield return ConnectionType.Cellular;
            break;
          case NetworkStatus.ReachableViaWiFiNetwork:
            yield return ConnectionType.WiFi;
            break;
          default:
            yield return ConnectionType.Other;
            break;
        }
      }
    }
    /// <summary>
    /// Not supported on iOS
    /// </summary>
    public IEnumerable<UInt64> Bandwidths
    {
      get;
      private set;
    }

  }
}