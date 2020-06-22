using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    public class LayuiTreeItem
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("children")]
        public List<LayuiTreeItem> Children { get; set; }

        [JsonPropertyName("href")]
        public string Url { get; set; }

        [JsonPropertyName("spread")]
        public bool Expand { get; set; }

        [JsonPropertyName("checked")]
        public bool Checked { get; set; }

        [JsonPropertyName("level")]
        public int Level { get; set; }
    }
}
