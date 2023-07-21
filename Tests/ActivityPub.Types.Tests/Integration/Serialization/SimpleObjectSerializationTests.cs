﻿// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

using ActivityPub.Types.Extended.Actor;
using ActivityPub.Types.Extended.Object;
using ActivityPub.Types.Tests.Util.Fixtures;
using ActivityPub.Types.Util;

namespace ActivityPub.Types.Tests.Integration.Serialization;

public abstract class SimpleObjectSerializationTests : SerializationTests
{
    public class EmptyObject : SimpleObjectSerializationTests
    {
        [Fact]
        public void ShouldWriteObject()
        {
            ObjectUnderTest = new ASObject();
            JsonUnderTest.ValueKind.Should().Be(JsonValueKind.Object);
        }
        
        [Fact]
        public void ShouldIncludeContext()
        {
            ObjectUnderTest = new ASObject();
            JsonUnderTest.Should().HaveProperty("@context");
            JsonUnderTest.GetProperty("@context").ValueKind.Should().Be(JsonValueKind.String);
            JsonUnderTest.GetProperty("@context").GetString().Should().Be("https://www.w3.org/ns/activitystreams");
        }

        [Fact]
        public void ShouldIncludeType()
        {
            ObjectUnderTest = new ASObject();
            JsonUnderTest.Should().HaveProperty("type");
            JsonUnderTest.GetProperty("type").ValueKind.Should().Be(JsonValueKind.String);
            JsonUnderTest.GetProperty("type").GetString().Should().Be("Object");
        }

        [Fact]
        public void ShouldIncludeOnlyTypeAndContext()
        {
            ObjectUnderTest = new ASObject();
            
            var props = JsonUnderTest.EnumerateObject().ToList();

            props.Should().HaveCount(2);
            props.Should().Contain(p => p.Name == "type");
            props.Should().Contain(p => p.Name == "@context");
        }
        
        public EmptyObject(JsonLdSerializerFixture fixture) : base(fixture) {}
    }

    public class Subclass : SimpleObjectSerializationTests
    {
        [Fact]
        public void ShouldSerializeToCorrectType()
        {
            ObjectUnderTest = new ImageObject();
            JsonUnderTest.GetProperty("type").GetString().Should().Be(ImageObject.ImageType);
        }

        [Fact]
        public void ShouldIncludePropertiesFromBaseTypes()
        {
            ObjectUnderTest = new PersonActor
            {
                // From ASActor
                Inbox = "https://example.com/actor/inbox",
                Outbox = "https://example.com/actor/outbox",

                // From ASObject
                Image = new ImageObject(),

                // From ASType
                Id = "https://example.com/actor/id"
            };

            JsonUnderTest.GetProperty("inbox").GetString().Should().Be("https://example.com/actor/inbox");
            JsonUnderTest.GetProperty("outbox").GetString().Should().Be("https://example.com/actor/outbox");
            JsonUnderTest.GetProperty("image").GetProperty("type").GetString().Should().Be("Image");
            JsonUnderTest.GetProperty("id").GetString().Should().Be("https://example.com/actor/id");
        }
        
        public Subclass(JsonLdSerializerFixture fixture) : base(fixture) {}
    }

    public class FullObject : SimpleObjectSerializationTests
    {
        [Fact]
        public void ShouldIncludeAllProperties()
        {
            ObjectUnderTest = new ASObject
            {
                // From ASObject
                Attachment = new LinkableList<ASObject> { new ASObject() },
                Audience = new LinkableList<ASObject> { new ASObject() },
                BCC = new LinkableList<ASObject> { new ASObject() },
                BTo = new LinkableList<ASObject> { new ASObject() },
                CC = new LinkableList<ASObject> { new ASObject() },
                Context = "https://example.com/some/context", // this is the worst field name
                Generator = new ASObject(),
                Icon = new ImageObject(),
                Image = new ImageObject(),
                InReplyTo = new ASObject(),
                Location = new ASObject(),
                Replies = new ASObject(),
                Tag = new() { new ASObject() },
                To = new() { new ASObject() },
                Url = "https://example.com",
                Content = new NaturalLanguageString("content"),
                Duration = "PT5S",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now,
                Published = DateTime.Now,
                Summary = new NaturalLanguageString("summary"),
                Updated = DateTime.Now,
                Source = new ASObject(),
                Likes = "https://example.com/likes.collection",
                Shares = "https://example.com/shares.collection",

                // From ASType
                Id = "https://example.com/some.uri",
                AttributedTo = new() { new ASObject() },
                Preview = new ASObject(),
                Name = new NaturalLanguageString("name"),
                MediaType = "text/html"
            };
            
            // From ASObject
            JsonUnderTest.TryGetProperty("attachment", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("audience", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("bcc", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("bto", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("cc", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("context", out _).Should().BeTrue(); // I hate this property NAME IT SOMETHING ELSE >:(
            JsonUnderTest.TryGetProperty("generator", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("icon", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("image", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("inReplyTo", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("location", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("replies", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("tag", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("to", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("url", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("content", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("duration", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("startTime", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("endTime", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("published", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("summary", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("updated", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("source", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("likes", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("shares", out _).Should().BeTrue();

            // From ASType
            JsonUnderTest.TryGetProperty("id", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("attributedTo", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("preview", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("name", out _).Should().BeTrue();
            JsonUnderTest.TryGetProperty("mediaType", out _).Should().BeTrue();
        }
        
        public FullObject(JsonLdSerializerFixture fixture) : base(fixture) {}
    }

    private SimpleObjectSerializationTests(JsonLdSerializerFixture fixture) : base(fixture) {}
}