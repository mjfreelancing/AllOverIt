﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace SerializeObjectPropertiesDemo
{
    internal class Dummy
    {
        public string Prop7 { get; set; }
        public int Prop8 { get; set; }
        public double Prop9 { get; set; }
        public bool Prop10 { get; set; }
        public Dummy Prop11 { get; set; }
        public Task<bool> Task { get; set; }
        public IEnumerable<int> Prop12 { get; set; }
    }
}