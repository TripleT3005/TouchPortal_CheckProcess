using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Events;
using TouchPortalSDK.Messages.Models;
using TouchPortalSDK.Messages.Models.Enums;

namespace TouchPortalSDK
{
    public class CheckProcess : ITouchPortalEventHandler
    {
        public string PluginId => "CheckProcess";

        private readonly ILogger<CheckProcess> _logger;
        private readonly ITouchPortalClient _client;

        private IReadOnlyCollection<Setting> _settings;

        public CheckProcess(ITouchPortalClientFactory clientFactory,
                            ILogger<CheckProcess> logger)
        {
            _logger = logger;
            _client = clientFactory.Create(this);
            _settings = new List<Setting>();
        }

        public void Run()
        {
            //Connect to Touch Portal:
            _client.Connect();

            //ToDo: Error without this?!
            _client.StateUpdate($"CheckProcess.Process1", "running");


            //CheckProcess
            int timerInterval = 10000; //10 second;
            var timerSetting = (_settings.FirstOrDefault(setting => setting.Name == "Update Interval (s)"))?.Value;
            if (!string.IsNullOrEmpty(timerSetting) && int.TryParse(timerSetting, out var parsedValue))
                timerInterval = parsedValue * 1000; //second to millisecond
            _logger?.LogInformation($"CheckProcess: timerInterval='{timerInterval}'");

            while (true)
            {
                int i = 0;
                foreach (var setting in _settings.Where(s => s.Name.StartsWith("Process")))
                {
                    i++;
                    var settingName = setting.Name;
                    var settingValue = setting.Value;

                    if (settingValue != null && settingValue != "")
                    {
                        if (checkprocess(settingValue))
                        {
                            _logger?.LogInformation($"CheckProcess: ProcessId='{i}', Name='{settingName}', Value='{settingValue}', Status='running'");
                            _client.StateUpdate($"CheckProcess.Process{i}", "running");
                        }
                        else
                        {
                            _logger?.LogInformation($"CheckProcess: ProcessId='{i}', Name='{settingName}', Value='{settingValue}', Status='not running'");
                            _client.StateUpdate($"CheckProcess.Process{i}", "not running");
                        }
                    }
                }
                Thread.Sleep(timerInterval);
            }
        }

        public bool checkprocess(string process)
        {
            Process[] processes = Process.GetProcessesByName(process);
            return processes.Length > 0;
        }

        public void OnClosedEvent(string message)
        {
            _logger?.LogInformation("TouchPortal Disconnected.");

            //Optional force exits this plugin.
            Environment.Exit(0);
        }

        /// <summary>
        /// Information received when plugin is connected to Touch Portal.
        /// </summary>
        /// <param name="message"></param>
        public void OnInfoEvent(InfoEvent message)
        {
            _logger?.LogInformation($"[Info] VersionCode: '{message.TpVersionCode}', VersionString: '{message.TpVersionString}', SDK: '{message.SdkVersion}', PluginVersion: '{message.PluginVersion}', Status: '{message.Status}'");

            _settings = message.Settings;
            _logger?.LogInformation($"[Info] Settings: {JsonSerializer.Serialize(_settings)}");
        }

        /// <summary>
        /// User selected an item in a dropdown menu in the Touch Portal UI.
        /// </summary>
        /// <param name="message"></param>
        public void OnListChangedEvent(ListChangeEvent message)
        {
            _logger?.LogInformation($"[OnListChanged] {message.ListId}/{message.ActionId}/{message.InstanceId} '{message.Value}'");

            switch (message.ListId)
            {
                //Dynamically updates the dropdown of data3 based on value chosen from data2 dropdown:
                case "category1.action1.data2" when message.InstanceId is not null:
                    var prefix = message.Value;
                    _client.ChoiceUpdate("category1.action1.data3", new[] { $"{prefix} second 1", $"{prefix} second 2", $"{prefix} second 3" }, message.InstanceId);
                    break;
            }
        }

        /// <summary>
        /// A broadcast event was received.
        /// </summary>
        /// <param name="message"></param>
        public void OnBroadcastEvent(BroadcastEvent message)
        {
            //Use this to reapply all state... Some times if you update the state, and the page is not visible, it will not be reflected in the app.
            _logger?.LogInformation($"[Broadcast] Event: '{message.Event}', PageName: '{message.PageName}'");
        }

        /// <summary>
        /// Updated settings was received.
        /// </summary>
        /// <param name="message"></param>
        public void OnSettingsEvent(SettingsEvent message)
        {
            _settings = message.Values;
            _logger?.LogInformation($"[OnSettings] Settings: {JsonSerializer.Serialize(_settings)}");
        }

        /// <summary>
        /// User clicked an action.
        /// </summary>
        /// <param name="message"></param>
        public void OnActionEvent(ActionEvent message)
        {
            switch (message.ActionId)
            {
                case "category1.action1":
                    //Get data with indexer:
                    var data1 = message["category1.action1.data1"] ?? "<null>";
                    var data2 = message["category1.action1.data2"] ?? "<null>";
                    var data3 = message["category1.action1.data3"] ?? "<null>";
                    var data4 = message["category1.action1.data4"] ?? "<null>";
                    //Get date with method:
                    var data5 = message.GetValue("category1.action1.data5") ?? "<null>";
                    var data6 = message.GetValue("category1.action1.data6") ?? "<null>";
                    var data7 = message.GetValue("category1.action1.data7") ?? "<null>";
                    var data8 = message.GetValue("category1.action1.data8") ?? "<null>";
                    _logger?.LogInformation($"[OnAction] PressState: {message.GetPressState()}, ActionId: {message.ActionId}, Data: data1:'{data1}', data2:'{data2}', data3:'{data3}', data4:'{data4}', data5:'{data5}', data6:'{data6}', data7:'{data7}', data8:'{data8}'");
                    break;

                default:
                    var dataArray = message.Data
                        .Select(dataItem => $"\"{dataItem.Key}\":\"{dataItem.Value}\"")
                        .ToArray();

                    var dataString = string.Join(", ", dataArray);
                    _logger?.LogInformation($"[OnAction] PressState: {message.GetPressState()}, ActionId: {message.ActionId}, Data: '{dataString}'");
                    break;
            }
        }

        /// <summary>
        /// Here you can react on what the person clicked in the option.
        /// </summary>
        /// <param name="message"></param>
        public void OnNotificationOptionClickedEvent(NotificationOptionClickedEvent message)
        {
            _logger?.LogInformation($"[OnNotificationOptionClickedEvent] NotificationId: '{message.NotificationId}', OptionId: '{message.OptionId}'");

            if (message.NotificationId is "TouchPortal.SamplePlugin|update")
            {
                switch (message.OptionId)
                {
                    //Example for opening a web browser (windows):
                    case "update":
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            UseShellExecute = true,
                            FileName = "https://www.nuget.org/packages/TouchPortalSDK/"
                        });
                        break;
                    case "readMore":
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            UseShellExecute = true,
                            FileName = "https://github.com/oddbear/TouchPortalSDK/"
                        });
                        break;
                }
            }
        }

        /// <summary>
        /// Event fired when the TP user moves a slider which uses one of this plugin's Connectors.
        /// This event is very similar to the OnActionEvent but with a `Value` attribute reflecting the slider's
        /// current position. Like actions, they may contain extra user-selected data.
        /// </summary>
        /// <param name="message"><see cref="ConnectorChangeEvent"/></param>
        public void OnConnecterChangeEvent(ConnectorChangeEvent message)
        {
            var dataArray = message.Data
                .Select(dataItem => $"\"{dataItem.Key}\":\"{dataItem.Value}\"")
                .ToArray();

            var dataString = string.Join(", ", dataArray);
            _logger?.LogInformation($"[OnConnecterChangeEvent] ConnectorId: '{message.ConnectorId}', Value: '{message.Value}', Data: '{dataString}'");
        }

        /// <summary>
        /// This event is generated when a TP user creates or modifies a slider which uses one of this plugin's Connectors.
        /// See the TP API documentation for usage details.
        /// </summary>
        /// <param name="message"><see cref="ShortConnectorIdNotificationEvent"/></param>
        public void OnShortConnectorIdNotificationEvent(ShortConnectorIdNotificationEvent message)
        {
            _logger?.LogInformation($"[OnShortConnectorIdNotificationEvent] ConnectorId: '{message.ConnectorId}', ShortID: '{message.ShortId}'");
        }

        /// <summary>
        /// The event was unknown and not handled, ex. a new version of TP is out with new features.
        /// </summary>
        /// <param name="jsonMessage"></param>
        public void OnUnhandledEvent(string jsonMessage)
        {
            _logger?.LogWarning($"Unhandled message: {jsonMessage}");
        }

    }
}