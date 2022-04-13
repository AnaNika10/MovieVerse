namespace Feed.Entities
{
    public class Comment
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string PostId { get; set; }
        public string Text { get; set; }
        public string[] Hashtags { get; set; }
        public int LikesNum { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
