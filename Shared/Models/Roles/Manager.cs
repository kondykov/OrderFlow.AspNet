﻿namespace OrderFlow.Shared.Models.Roles;

public sealed class Manager : Role
{
    public Manager()
    {
        Name = "Manager";
        NormalizedName = Name.ToUpper();
        ParentRole = new Admin().ToString();
    }
}