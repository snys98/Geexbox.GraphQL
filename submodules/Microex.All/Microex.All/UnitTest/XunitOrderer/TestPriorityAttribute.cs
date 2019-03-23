using System;

namespace Microex.All.UnitTest.XunitOrderer
{
    public class TestPriorityAttribute : Attribute
    {
        public TestPriorityAttribute(double priority) => Priority = priority;

        public double Priority { get; }
    }
}
