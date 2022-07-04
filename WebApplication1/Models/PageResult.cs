using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class PageResult<T>
    {
        public List<T> Items { get; set; }
        public int Totalpages { get; set; }
        public int ItemFrom { get; set; }
        public int ItemsTo { get; set; }
        public int TotalItemsCount { get; set; }

        public PageResult(List<T> items, int pageNumber, int pageSize, int totalItemsCount)
        {
            Items = items;
            ItemFrom = pageSize*(pageNumber-1)+1;
            ItemsTo = ItemFrom+pageSize-1;
            TotalItemsCount = totalItemsCount;
            Totalpages = (int)Math.Ceiling(totalItemsCount /(double) pageSize);
        }
    }
}
