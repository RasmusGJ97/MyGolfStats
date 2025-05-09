namespace MyGolfStatsApi.DTOs
{
    public class BagUpdateDTO
    {
        public Guid UserId { get; set; }
        public bool Driver { get; set; } = true;
        public bool TwoW { get; set; } = true;
        public bool ThreeW { get; set; } = true;
        public bool FourW { get; set; } = true;
        public bool FiveW { get; set; } = true;
        public bool SixW { get; set; } = true;
        public bool SevenW { get; set; } = true;
        public bool TwoHy { get; set; } = true;
        public bool ThreeHy { get; set; } = true;
        public bool FourHy { get; set; } = true;
        public bool FiveHy { get; set; } = true;
        public bool OneI { get; set; } = true;
        public bool TwoI { get; set; } = true;
        public bool ThreeI { get; set; } = true;
        public bool FourI { get; set; } = true;
        public bool FiveI { get; set; } = true;
        public bool SixI { get; set; } = true;
        public bool SevenI { get; set; } = true;
        public bool EightI { get; set; } = true;
        public bool NineI { get; set; } = true;
        public bool AWedge { get; set; } = true;
        public bool GWedge { get; set; } = true;
        public bool SWedge { get; set; } = true;
        public bool LWedge { get; set; } = true;
    }
}
