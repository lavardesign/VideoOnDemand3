using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace VOD.Common.Entities
{
    public class UserCourse
    {
        public string UserId { get; set; }
        public VODUser User { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
