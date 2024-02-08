using Newtonsoft.Json;

namespace QsysDiscovery
{
    public class DiscoveredDevice
    {
        [JsonProperty("ref")] public string Ref { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("part_number")] public string PartNumber { get; set; }
        [JsonProperty("platform")] public string Platform { get; set; }
        [JsonProperty("lan_a_ip")] public string IpAddress { get; set; }
        [JsonProperty("lan_b_ip")] public string IpAddressLanB { get; set; }
        [JsonProperty("aux_a_ip")] public string IpAddressAux { get; set; }
        [JsonProperty("lan_a_lldp")] public string LldpInfo { get; set; }
        [JsonProperty("lan_b_lldp")] public string LldpInfoLanB { get; set; }
        [JsonProperty("web_cfg_url")] public string WebConfigUrl { get; set; }
        [JsonProperty("secure_comm")] public string SecureComm { get; set; }
        [JsonProperty("is_virtual")] public bool? IsVirtual { get; set; }
        [JsonProperty("https_server_up")] public bool? HttpsServerUp { get; set; }
        public DiscoveredControlInfo ControlInfo { get; set; }
    }

    public class DiscoveredControlInfo
    {
        [JsonProperty("ref")] public string Ref { get; set; }
        [JsonProperty("role")] public string Role { get; set; }
        [JsonProperty("device_ref")] public string DeviceRef { get; set; }
        [JsonProperty("design_pretty")] public string DesignName { get; set; }
        [JsonProperty("design_code")] public string DesignCode { get; set; }
        [JsonProperty("primary")] public int Primary { get; set; }
        [JsonProperty("redundant")] public int Redundant { get; set; }
    }
}