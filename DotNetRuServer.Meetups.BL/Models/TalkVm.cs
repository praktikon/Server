using System.Collections.Generic;

namespace DotNetRuServer.Meetups.BL.Models
{
    public class TalkVm
    {
        public string Id { get; set; }

        public List<string> SpeakerIds { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string CodeUrl { get; set; }

        public string SlidesUrl { get; set; }

        public string VideoUrl { get; set; }
    }
}