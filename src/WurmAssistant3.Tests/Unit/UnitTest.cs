using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Telerik.JustMock.AutoMock;

namespace AldursLab.WurmAssistant3.Tests.Unit
{
    abstract class UnitTest<TService> : AssertionHelper where TService : class
    {
        MockingContainer<TService> kernel;

        [SetUp]
        public void BaseSetup()
        {
            kernel = new MockingContainer<TService>();
        }

        public MockingContainer<TService> Kernel
        {
            get { return kernel; }
        }

        public TService Service
        {
            get { return kernel.Instance; }
        }
    }
}
