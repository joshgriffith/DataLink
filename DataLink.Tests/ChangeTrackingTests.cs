using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLink.Core.ChangeTracking;
using DataLink.Core.Interceptors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataLink.Tests {
    
    [TestClass]
    public class ChangeTrackingTests {

        private IsChangeTracker GetChangeTracker() {
            return new SnapshotChangeTracker();
        }

        [TestMethod]
        public void Should_Have_NoChanges() {
            var tracker = GetChangeTracker();

            var item = new TestItem();
            tracker.Add(item);

            Assert.IsFalse(tracker.HasChanges());
        }

        [TestMethod]
        public void Should_HaveChanged_SimpleProperty() {
            var tracker = GetChangeTracker();
            tracker.Add(new TestItem2());

            var item = new TestItem();
            tracker.Add(item);
            item.StringProperty = "test";

            var changes = tracker.GetChanges();

            Assert.AreEqual(1, changes.Entries.Count);
        }

        [TestMethod]
        public void Should_HaveChanged_ComplexProperty() {
            var tracker = GetChangeTracker();
            tracker.Add(new TestItem2());

            var item = new TestItem();
            tracker.Add(item);

            item.NavigationProperty = new TestItem {
                StringProperty = "foo"
            };

            var changes = tracker.GetChanges();

            Assert.AreEqual(1, changes.Entries.Count);
        }

        [TestMethod]
        public void Should_HaveNotChanged_DictionaryEntry() {
            var tracker = GetChangeTracker();

            var item = new TestItem();
            item.DictionaryProperty["foo"] = new List<object>();
            tracker.Add(item);
            
            var changes = tracker.GetChanges();

            Assert.AreEqual(0, changes.Entries.Count);
        }

        [TestMethod]
        public void ShouldNot_HaveChanged_ReplaceReference() {
            var tracker = GetChangeTracker();

            var item = new TestItem {
                NavigationProperty = new TestItem {
                    IntegerProperty = 1
                }
            };

            tracker.Add(item);

            item.NavigationProperty = new TestItem {
                IntegerProperty = 1
            };

            var changes = tracker.GetChanges();

            Assert.AreEqual(0, changes.Entries.Count);
        }

        [TestMethod]
        public void Should_HaveChanged_ReplaceReference() {
            var tracker = GetChangeTracker();

            var item = new TestItem {
                NavigationProperty = new TestItem {
                    IntegerProperty = 1
                }
            };

            tracker.Add(item);

            item.NavigationProperty = new TestItem {
                IntegerProperty = 2
            };

            var changes = tracker.GetChanges();

            Assert.AreEqual(1, changes.Entries.Count);
        }

        [TestMethod]
        public void Should_HaveChanged_ReplaceCollection() {
            var tracker = GetChangeTracker();

            var item = new TestItem {
                CollectionProperty = new List<TestItem> {
                    new()
                }
            };

            tracker.Add(item);

            item.CollectionProperty = new List<TestItem>();

            var changes = tracker.GetChanges();

            Assert.AreEqual(1, changes.Entries.Count);
        }

        [TestMethod]
        public void Should_Add_ListItem() {
            var tracker = GetChangeTracker();

            var item = new TestItem();
            tracker.Add(item);

            item.CollectionProperty.Add(new TestItem {
                StringProperty = "foo"
            });

            var changes = tracker.GetChanges();

            Assert.AreEqual(1, changes.Entries.Count);
            Assert.AreEqual(1, changes.Entries.First().GetChanges(ChangeTypes.Create).Count());
        }

        [TestMethod]
        public void Should_Delete_ListItem() {
            var tracker = GetChangeTracker();

            var item = new TestItem();

            item.CollectionProperty.Add(new TestItem {
                StringProperty = "foo"
            });

            tracker.Add(item);

            item.CollectionProperty.Remove(item.CollectionProperty.First());

            var changes = tracker.GetChanges();

            Assert.AreEqual(1, changes.Entries.Count);
            Assert.AreEqual(1, changes.Entries.First().GetChanges(ChangeTypes.Delete).Count());
        }

        [TestMethod]
        public void Should_Change_ListItem() {
            var tracker = GetChangeTracker();

            var item = new TestItem();

            item.CollectionProperty.Add(new TestItem {
                StringProperty = "foo"
            });

            tracker.Add(item);

            item.CollectionProperty[0].StringProperty = "bar";

            var changes = tracker.GetChanges();

            Assert.AreEqual(1, changes.Entries.Count);
            Assert.AreEqual(1, changes.Entries.First().GetChanges(ChangeTypes.Update).Count());
        }

        [TestMethod]
        public async Task ShouldIntercept_SimpleProperty() {
            var tracker = GetChangeTracker();

            var item = new TestItem();
            tracker.Add(item);
            item.StringProperty = "test";

            var changes = tracker.GetChanges();

            var interceptor = new ChangeInterceptor<TestItem>();
            var intercepted = false;

            interceptor.On(x => x.StringProperty, (i, v) => {
                intercepted = true;
                Assert.AreEqual("test", v);
                Assert.AreEqual(item, i);
                return Task.CompletedTask;
            });

            await interceptor.BeforeCommit(changes);

            Assert.IsTrue(intercepted);
        }

        [TestMethod]
        public async Task ShouldIntercept_ComplexProperty() {
            var tracker = GetChangeTracker();

            var item = new TestItem();
            item.CollectionProperty.Add(new TestItem());
            item.CollectionProperty.Add(new TestItem());

            tracker.Add(item);
            item.NavigationProperty = new TestItem {
                IntegerProperty = 1
            };

            var changes = tracker.GetChanges();

            var interceptor = new ChangeInterceptor<TestItem>();
            var intercepted = false;

            interceptor.On(x => x.NavigationProperty, (i, v) => {
                intercepted = true;
                Assert.AreEqual(item.NavigationProperty, v);
                Assert.AreEqual(item, i);
                return Task.CompletedTask;
            });

            await interceptor.BeforeCommit(changes);

            Assert.IsTrue(intercepted);
        }

        [TestMethod]
        public async Task ShouldIntercept_AddedItem() {
            var tracker = GetChangeTracker();

            var item = new TestItem();
            item.CollectionProperty.Add(new TestItem { IntegerProperty = 1 });

            tracker.Add(item);
            item.CollectionProperty.Add(new TestItem { IntegerProperty = 2 });
            item.CollectionProperty.Add(new TestItem { IntegerProperty = 3 });
            
            var interceptor = new ChangeInterceptor<TestItem>();
            var intercepts = 0;

            interceptor.OnAdd(x => x.CollectionProperty, (i, v) => {
                intercepts += 1;
                return Task.CompletedTask;
            });

            var changes = tracker.GetChanges();
            await interceptor.BeforeCommit(changes);

            Assert.AreEqual(2, intercepts);
        }

        [TestMethod]
        public async Task ShouldIntercept_RemovedItem() {
            var tracker = GetChangeTracker();

            var item = new TestItem();
            var child = new TestItem { IntegerProperty = 1 };
            item.CollectionProperty.Add(child);

            tracker.Add(item);
            item.CollectionProperty.Add(new TestItem { IntegerProperty = 2 });
            item.CollectionProperty.Remove(child);

            var changes = tracker.GetChanges();

            var interceptor = new ChangeInterceptor<TestItem>();
            var intercepted = false;

            interceptor.OnRemove(x => x.CollectionProperty, (i, v) => {
                intercepted = true;
                return Task.CompletedTask;
            });

            await interceptor.BeforeCommit(changes);

            Assert.IsTrue(intercepted);
        }

        protected class TestItem {

            public TestItem() {
                CollectionProperty = new List<TestItem>();
                DictionaryProperty = new Dictionary<string, object>();
            }
            
            public int IntegerProperty { get; set; }
            public string StringProperty { get; set; }
            public TestItem NavigationProperty { get; set; }
            public List<TestItem> CollectionProperty { get; set; }
            public Dictionary<string, object> DictionaryProperty { get; set; }

            public override bool Equals(object? obj) {
                if (obj is TestItem item)
                    return item.IntegerProperty == IntegerProperty;

                return base.Equals(obj);
            }
        }

        protected class TestItem2 {
            public bool BooleanProperty { get; set; }
        }
    }
}