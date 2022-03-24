using System.Collections.Generic;
using System.Linq;
using DataLink.Core.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataLink.Tests {

    [TestClass]
    public class UtilityTests {
        
        [TestMethod]
        public void Should_CamelcaseDictionaryProperties() {
            var target = new UtilityTestClass {
                Data = {
                    ["Foo"] = "bar",
                    ["Test"] = new Dictionary<string, object> {
                        { "Key1", "value1" }
                    },
                    ["abc"] = "123"
                },
                ListProperty = new List<object> {
                    new Dictionary<string, object> {
                        { "A", "b" }
                    }
                }
            };

            var visitor = new CamelcaseDictionaryVisitor();
            visitor.Visit(target);

            Assert.IsTrue(target.Data.ContainsKey("foo"));
            Assert.IsFalse(target.Data.ContainsKey("Foo"));
            Assert.AreEqual("bar", target.Data["foo"]);
            Assert.IsTrue(target.Data.ContainsKey("test"));

            var nested = target.Data["test"] as Dictionary<string, object>;

            Assert.IsTrue(nested.ContainsKey("key1"));

            var dictionary = target.ListProperty.First() as Dictionary<string, object>;
            Assert.IsTrue(dictionary.ContainsKey("a"));
        }

        protected class UtilityTestClass {
            public List<object> ListProperty { get; set; }
            public Dictionary<string, object> Data = new();
        }
    }
}