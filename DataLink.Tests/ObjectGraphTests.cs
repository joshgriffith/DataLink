using System.Collections.Generic;
using System.Linq;
using DataLink.Core.ChangeTracking.Graph;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataLink.Tests {

    [TestClass]
    public class ObjectGraphTests {

        [TestMethod]
        public void Should_BuildGraph() {
            var graph = new ObjectGraph(new TestObject {
                StringProperty = "foo",
                IntProperty = 2,
                NavigationProperty = new TestObject {
                    IntProperty = 3
                },
                CollectionProperty = new List<TestObject> {
                    new() { IntProperty = 4 },
                    new() { IntProperty = 5 }
                }
            });

            var stringProperty = graph.Properties.First();
            Assert.AreEqual(nameof(TestObject.StringProperty), stringProperty.Name);
            Assert.AreEqual("foo", stringProperty.Value);

            var navigationProperty = graph.Properties.First(each => each.Name == nameof(TestObject.NavigationProperty)) as ObjectGraph;
            Assert.IsInstanceOfType(navigationProperty.Value, typeof(TestObject));

            var intProperty = navigationProperty.Properties.First(each => each.Name == nameof(TestObject.IntProperty));
            Assert.AreEqual(nameof(TestObject.IntProperty), intProperty.Name);
            Assert.AreEqual(3, intProperty.Value);

            var collectionProperty = graph.Properties.First(each => each.Name == nameof(TestObject.CollectionProperty)) as ObjectGraphCollection;
            Assert.AreEqual(nameof(TestObject.CollectionProperty), collectionProperty.Name);
            Assert.AreEqual(2, collectionProperty.Items.Count);
            Assert.AreEqual("0", collectionProperty.Items.First().Name);
        }

        protected class TestObject {
            public string StringProperty { get; set; }
            public int IntProperty { get; set; }
            public TestObject NavigationProperty { get; set; }
            public List<TestObject> CollectionProperty { get; set; }
        }
    }
}