using System;
using System.Collections.Generic;

namespace Feed.DTOs.Post
{
    public class BasePostDTO
    {
        public string UserId { get; set; }
        public string PostText { get; set; }
        public string[] Hashtags { get; set; }
        public int NumOfLikes { get; set; }
        public string[] FilesUrls { get; set; }
        //   public Comment[] ListOfComments { get; set;}
        public DateTime CreatedDate { get; set; }

 

    }
}
