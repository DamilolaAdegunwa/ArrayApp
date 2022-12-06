﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Domain.Entities.AdvertAggregate;

namespace ArrayApp.Domain.Entities.CommentAggregate;
public class Comment
{
    // The comment's text
    public string Text { get; set; }

    // The date and time the comment was created
    public DateTime CreatedAt { get; set; }

    // The user who created the comment
    public User Creator { get; set; }

    // The comment's rating (if it has one)
    public int Rating { get; set; }

    // The comment's status (e.g. "pending" or "approved")
    public string Status { get; set; }

    // The comment's parent (if it is a reply to another comment)
    public Comment Parent { get; set; }

    // The post, page, or other content that the comment is associated with
    public Content Content { get; set; }
}