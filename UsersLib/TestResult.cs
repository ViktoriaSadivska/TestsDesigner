﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DBLib
{
    [Serializable]
    public class TestResult
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public double Points { get; set; }
        public bool IsPassed { get; set; }
    }
}
