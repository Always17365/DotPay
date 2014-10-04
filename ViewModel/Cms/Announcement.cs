using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using DotPay.Common;
using FC.Framework;

namespace DotPay.ViewModel
{
    public class AnnouncementModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsTop { get; set; }
    }
    
    public class ArticleInListModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsTop { get; set; }
        public int CreateAt { get; set; }
        public Lang Lang { get; set; }
        public string CNCategory { get { return this.Category.GetDescription(); } }
        public ArticleCategory Category { get; set; }
        public string CreateBy { get; set; }
        public string UpdateAt { get; set; }
        public string UpdateBy { get; set; }
    }
    public class NoticeInListModel
    {
        public int ID{get;set;}
        public string Title { get;set;}
        public string Content { get; set; }
        public bool IsTop { get; set; }
        public Lang Lang { get; set; }
        public int CreateAt { get; set; }
        public string CreateBy { get; set; }
        public string UpdateAt { get; set; }
        public string UpdateBy { get; set; }
    }
}
