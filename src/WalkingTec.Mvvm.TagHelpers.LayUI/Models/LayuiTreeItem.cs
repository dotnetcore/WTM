using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    public class LayuiTreeItem
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("children")]
        public List<LayuiTreeItem> Children { get; set; }

        [JsonProperty("href")]
        public string Url { get; set; }

        [JsonProperty("spread")]
        public bool Expand { get; set; }

        [JsonProperty("checked")]
        public bool Checked { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }
    }
}
