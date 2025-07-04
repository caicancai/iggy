﻿// // Licensed to the Apache Software Foundation (ASF) under one
// // or more contributor license agreements.  See the NOTICE file
// // distributed with this work for additional information
// // regarding copyright ownership.  The ASF licenses this file
// // to you under the Apache License, Version 2.0 (the
// // "License"); you may not use this file except in compliance
// // with the License.  You may obtain a copy of the License at
// //
// //   http://www.apache.org/licenses/LICENSE-2.0
// //
// // Unless required by applicable law or agreed to in writing,
// // software distributed under the License is distributed on an
// // "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// // KIND, either express or implied.  See the License for the
// // specific language governing permissions and limitations
// // under the License.

using Apache.Iggy.Contracts.Http;
using Apache.Iggy.Enums;
using Apache.Iggy.Exceptions;
using Apache.Iggy.Tests.Integrations.Fixtures;
using Shouldly;

namespace Apache.Iggy.Tests.Integrations;

[MethodDataSource<IggyServerFixture>(nameof(IggyServerFixture.ProtocolData))]
public class PartitionsTests(Protocol protocol)
{
    [ClassDataSource<PartitionsFixture>(Shared = SharedType.PerClass)]
    public required PartitionsFixture Fixture { get; init; }

    [Test]
    public async Task CreatePartition_HappyPath_Should_CreatePartition_Successfully()
    {
        await Should.NotThrowAsync(() => Fixture.Clients[protocol].CreatePartitionsAsync(new CreatePartitionsRequest
        {
            PartitionsCount = 3,
            StreamId = Identifier.Numeric(Fixture.StreamRequest.StreamId!.Value),
            TopicId = Identifier.Numeric(Fixture.TopicRequest.TopicId!.Value)
        }));

        var response = await Fixture.Clients[protocol].GetTopicByIdAsync(Identifier.Numeric(1), Identifier.Numeric(1));
        response.ShouldNotBeNull();
        response.PartitionsCount.ShouldBe(4);
    }

    [Test]
    [DependsOn(nameof(CreatePartition_HappyPath_Should_CreatePartition_Successfully))]
    public async Task DeletePartition_Should_DeletePartition_Successfully()
    {
        await Should.NotThrowAsync(() => Fixture.Clients[protocol].DeletePartitionsAsync(new DeletePartitionsRequest
        {
            PartitionsCount = 1,
            StreamId = Identifier.Numeric(Fixture.StreamRequest.StreamId!.Value),
            TopicId = Identifier.Numeric(Fixture.TopicRequest.TopicId!.Value)
        }));

        var response = await Fixture.Clients[protocol].GetTopicByIdAsync(Identifier.Numeric(1), Identifier.Numeric(1));
        response.ShouldNotBeNull();
        response.PartitionsCount.ShouldBe(3);
    }

    [Test]
    [DependsOn(nameof(DeletePartition_Should_DeletePartition_Successfully))]
    public async Task DeletePartition_Should_Throw_WhenTopic_DoesNotExist()
    {
        await Fixture.Clients[protocol].DeleteTopicAsync(Identifier.Numeric(1), Identifier.Numeric(1));
        await Should.ThrowAsync<InvalidResponseException>(() => Fixture.Clients[protocol].DeletePartitionsAsync(new DeletePartitionsRequest
        {
            PartitionsCount = 1,
            StreamId = Identifier.Numeric(Fixture.StreamRequest.StreamId!.Value),
            TopicId = Identifier.Numeric(Fixture.TopicRequest.TopicId!.Value)
        }));
    }

    [Test]
    [DependsOn(nameof(DeletePartition_Should_Throw_WhenTopic_DoesNotExist))]
    public async Task DeletePartition_Should_Throw_WhenStream_DoesNotExist()
    {
        await Fixture.Clients[protocol].DeleteStreamAsync(Identifier.Numeric(1));
        await Should.ThrowAsync<InvalidResponseException>(() => Fixture.Clients[protocol].DeletePartitionsAsync(new DeletePartitionsRequest
        {
            PartitionsCount = 1,
            StreamId = Identifier.Numeric(Fixture.StreamRequest.StreamId!.Value),
            TopicId = Identifier.Numeric(Fixture.TopicRequest.TopicId!.Value)
        }));
    }
}