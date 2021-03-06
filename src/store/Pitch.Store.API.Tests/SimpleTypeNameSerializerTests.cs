﻿using System;
using Pitch.Store.API.Supporting;
using Xunit;

namespace Pitch.Store.API.Tests
{
    public class SimpleTypeNameSerializerTest
    {
        public class TestObject {}

        [Fact]
        public void DeSerialize_ReturnsCorrectType()
        {
            //Arrange
            var types = new Type[] { typeof(TestObject) };

            //Act
            var serializer = new SimpleTypeNameSerializer(types);

            //Assert
            Assert.Equal(typeof(TestObject), serializer.DeSerialize("TestObject"));
        }

        [Fact]
        public void Serialize_ReturnsCorrectString()
        {
            //Arrange
            var types = new Type[] { typeof(TestObject) };

            //Act
            var serializer = new SimpleTypeNameSerializer(types);

            //Assert
            Assert.Equal("TestObject", serializer.Serialize(typeof(TestObject)));
        }
    }
}
