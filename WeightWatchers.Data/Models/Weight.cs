﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace WeightWatchers.Data.Models;

public partial class Weight
{
    public DateOnly Date { get; set; }

    public int PersonId { get; set; }

    public decimal Lbs { get; set; }

    public decimal Kgs { get; set; }

    public virtual Person Person { get; set; }
}