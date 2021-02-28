using System;

namespace RoadStatusClient.Model
{
    public class RoadStatusDto
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Group { get; set; }
        public string StatusSeverity { get; set; }
        public string StatusSeverityDescription { get; set; }
        public string Bounds { get; set; }
        public string Envelope { get; set; }
        public DateTime? StatusAggregationStartDate { get; set; }
        public DateTime? StatusAggregationEndDate { get; set; }
        public string Url { get; set; }
    }
}
