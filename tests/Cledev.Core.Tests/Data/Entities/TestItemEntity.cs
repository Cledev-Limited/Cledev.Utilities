﻿using Cledev.Core.Data;

namespace Cledev.Core.Tests.Data.Entities;

public sealed class TestItemEntity : Entity
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    
    public List<TestSubItemEntity> TestSubItems { get; set; } = new();
}