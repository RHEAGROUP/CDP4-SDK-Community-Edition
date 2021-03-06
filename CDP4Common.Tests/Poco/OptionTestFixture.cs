﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionTestFixture.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2020 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft
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

namespace CDP4Common.Tests.Poco
{
    using System;
    using System.Linq;

    using CDP4Common.EngineeringModelData;
    using CDP4Common.Tests.Helpers;

    using NUnit.Framework;

    [TestFixture]
    internal class OptionTestFixture
    {
        [Test]
        public void VerifyThatIsDefaultWorks()
        {
            var iteration = new Iteration();
            var option1 = new Option();
            var option2 = new Option();

            iteration.Option.Add(option1);
            iteration.Option.Add(option2);
            iteration.DefaultOption = option1;

            Assert.IsTrue(option1.IsDefault);
            Assert.IsFalse(option2.IsDefault);
        }

        [Test]
        public void VerifyGetNestedParameterValuesByPath()
        {
            var nestedElementTreeGeneratorTestFixture = new NestedElementTreeGeneratorTestFixture();
            nestedElementTreeGeneratorTestFixture.SetUp();

            var option = nestedElementTreeGeneratorTestFixture.iteration.Option.Single(x => x.ShortName == "OPT_A");

            var doubleParameters = option.GetNestedParameterValuesByPath<double>(@"Sat\m\\OPT_A", nestedElementTreeGeneratorTestFixture.domainOfExpertise).ToList();
            Assert.AreEqual(1, doubleParameters.Count);
            Assert.AreEqual(3D, doubleParameters.First());

            var stringParameters = option.GetNestedParameterValuesByPath<string>(@"Sat\m\\OPT_A", nestedElementTreeGeneratorTestFixture.domainOfExpertise).ToList();
            Assert.AreEqual(1, stringParameters.Count);
            Assert.AreEqual("3", stringParameters.First());

            Assert.Throws<FormatException>(() => option.GetNestedParameterValuesByPath<bool>(@"Sat\m\\OPT_A", nestedElementTreeGeneratorTestFixture.domainOfExpertise));

            var objectParameters = option.GetNestedParameterValuesByPath<object>(@"Sat\m\\OPT_A", nestedElementTreeGeneratorTestFixture.domainOfExpertise).ToList();
            Assert.AreEqual(1, objectParameters.Count);
            Assert.AreEqual(3D, Convert.ChangeType(objectParameters.Single(), typeof(double)));
            Assert.AreEqual("3", Convert.ChangeType(objectParameters.Single(), typeof(string)));

            var domainCheckParameters = option.GetNestedParameterValuesByPath<double>(@"Sat.bat_b\v\1\OPT_A", nestedElementTreeGeneratorTestFixture.domainOfExpertise).ToList();
            Assert.AreEqual(0, domainCheckParameters.Count);

            var domainCheckParametersWithCorrectDomain = option.GetNestedParameterValuesByPath<double>(@"Sat.bat_b\v\1\OPT_A", nestedElementTreeGeneratorTestFixture.domainOfExpertise_2).ToList();
            Assert.AreEqual(1, domainCheckParametersWithCorrectDomain.Count);
            Assert.AreEqual(220D, domainCheckParametersWithCorrectDomain.First());

            var domain2CheckParameters = option.GetNestedParameterValuesByPath<double>(@"Sat.bat_b\v\1\OPT_A").ToList();
            Assert.AreEqual(1, domain2CheckParameters.Count);
            Assert.AreEqual(220D, domain2CheckParameters.First());

            var defaultValueCheckParameter = option.GetNestedParameterValuesByPath<double>(@"Sat.bat_b\v\2\OPT_A").ToList();
            Assert.AreEqual(1, defaultValueCheckParameter.Count);
            Assert.AreEqual(0D, defaultValueCheckParameter.First());
        }

        [Test]
        public void VerifyGetNestedPublishedParameterValuesByPath()
        {
            var nestedElementTreeGeneratorTestFixture = new NestedElementTreeGeneratorTestFixture();
            nestedElementTreeGeneratorTestFixture.SetUp();

            var option = nestedElementTreeGeneratorTestFixture.iteration.Option.Single(x => x.ShortName == "OPT_A");

            var doubleParameters = option.GetNestedParameterPublishedValuesByPath<double>(@"Sat\m\\OPT_A", nestedElementTreeGeneratorTestFixture.domainOfExpertise).ToList();
            Assert.AreEqual(1, doubleParameters.Count);
            Assert.AreEqual(123, doubleParameters.Single());

            var stringParameters = option.GetNestedParameterPublishedValuesByPath<string>(@"Sat\m\\OPT_A", nestedElementTreeGeneratorTestFixture.domainOfExpertise).ToList();
            Assert.AreEqual(1, stringParameters.Count);
            Assert.AreEqual("123", stringParameters.Single());

            Assert.Throws<FormatException>(() => option.GetNestedParameterPublishedValuesByPath<bool>(@"Sat\m\\OPT_A", nestedElementTreeGeneratorTestFixture.domainOfExpertise));

            var objectParameters = option.GetNestedParameterPublishedValuesByPath<object>(@"Sat\m\\OPT_A", nestedElementTreeGeneratorTestFixture.domainOfExpertise).ToList();
            Assert.AreEqual(1, objectParameters.Count);
            Assert.AreEqual(123, Convert.ChangeType(objectParameters.Single(), typeof(double)));
            Assert.AreEqual("123", Convert.ChangeType(objectParameters.Single(), typeof(string)));

            var domainCheckParameters = option.GetNestedParameterPublishedValuesByPath<double>(@"Sat.bat_b\v\1\OPT_A", nestedElementTreeGeneratorTestFixture.domainOfExpertise).ToList();
            Assert.AreEqual(0, domainCheckParameters.Count);

            var domainCheckParametersWithCorrectDomain = option.GetNestedParameterPublishedValuesByPath<double>(@"Sat.bat_b\v\1\OPT_A", nestedElementTreeGeneratorTestFixture.domainOfExpertise_2).ToList();
            Assert.AreEqual(1, domainCheckParametersWithCorrectDomain.Count);
            Assert.AreEqual(0D, domainCheckParametersWithCorrectDomain.Single());

            var domain2CheckParameters = option.GetNestedParameterPublishedValuesByPath<double>(@"Sat.bat_b\v\1\OPT_A").ToList();
            Assert.AreEqual(1, domain2CheckParameters.Count);
            Assert.AreEqual(0D, domain2CheckParameters.Single());

            var defaultValueCheckParameter = option.GetNestedParameterPublishedValuesByPath<double>(@"Sat.bat_b\v\2\OPT_A").ToList();
            Assert.AreEqual(1, defaultValueCheckParameter.Count);
            Assert.AreEqual(123D, defaultValueCheckParameter.Single());
        }
    }
}
