﻿using System.Collections.Generic;

namespace BiblioTECH.Models.Branch
{
    public class BranchDetailModel
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string BranchName { get; set; }
        public string BranchOpenedDate { get; set; }
        public string Telephone { get; set; }
        public bool IsOpen { get; set; }
        public string Description { get; set; }
        public int NumberOfPatrons { get; set; }
        public int NumberOfAssets { get; set; }
        public float TotalAssetValue { get; set; }
        public string ImageUrl { get; set; }

        //E pentru afisare
        public int Index { get; set; }
        public IEnumerable<string> HoursOpen { get; set; }
    }
}
