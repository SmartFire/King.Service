﻿namespace King.Service.Tests
{
    using King.Azure.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ProcHelper : IProcessor<int>
    {
        public static int Testing = 0;

        public async Task<bool> Process(int data)
        {
            Testing = data;

            return await Task.FromResult(true);
        }
    }
}