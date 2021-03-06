﻿#region Copyright
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PrefixedUnitTestFixture.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2019 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou
//
//    This file is part of CDP4-SDK Community Edition
//
//    The CDP4-SDK Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4-SDK Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public License
//    along with this program; if not, write to the Free Software Foundation,
//    Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace CDP4Common.Tests.Poco
{
    using CDP4Common.SiteDirectoryData;
    using NUnit.Framework;

    [TestFixture]
    internal class PrefixedUnitTestFixture
    {
        private PrefixedUnit prefixedUnit;
        private UnitPrefix unitPrefix;
        private MeasurementUnit measurementUnit;

        [SetUp]
        public void Setup()
        {
            this.prefixedUnit = new PrefixedUnit();

            this.unitPrefix = new UnitPrefix();
            this.unitPrefix.ConversionFactor = "conv";
            this.unitPrefix.Name = "unit";
            this.unitPrefix.ShortName = "u";

            this.measurementUnit = new SimpleUnit();
            this.measurementUnit.Name = "measurement";
            this.measurementUnit.ShortName = "m";
        }

        [Test]
        public void TestGetConversion()
        {
            this.prefixedUnit.Prefix = this.unitPrefix;
            Assert.AreEqual("conv", this.prefixedUnit.ConversionFactor);
        }

        [Test]
        public void TestGetConversionEmpty()
        {
            Assert.IsEmpty(this.prefixedUnit.ConversionFactor);
        }

        [Test]
        public void TestGetName()
        {
            this.prefixedUnit.Prefix = this.unitPrefix;
            this.prefixedUnit.ReferenceUnit = this.measurementUnit;
            Assert.AreEqual("unitmeasurement", this.prefixedUnit.Name);
        }

        [Test]
        public void TestGetNameEmpty()
        {
            Assert.IsEmpty(this.prefixedUnit.Name);
        }

        [Test]
        public void TestGetShortName()
        {
            this.prefixedUnit.Prefix = this.unitPrefix;
            this.prefixedUnit.ReferenceUnit = this.measurementUnit;
            Assert.AreEqual("um", this.prefixedUnit.ShortName);
        }

        [Test]
        public void TestGetShortNameEmpty()
        {
            Assert.IsEmpty(this.prefixedUnit.ShortName);
        }
    }
}