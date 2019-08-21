﻿// <copyright file="CategorizableThingRuleChecker.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2019 RHEA System S.A.
//
//    Author: Sam Gerené
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

namespace CDP4Rules.RuleCheckers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using CDP4Common.CommonData;
    using CDP4Common.SiteDirectoryData;
    using CDP4Rules.Common;

    /// <summary>
    /// The purpose of the <see cref="CategorizableThingRuleChecker"/> is to execute the rules for instances of type <see cref="ICategorizableThing"/>
    /// </summary>
    [RuleChecker(typeof(ICategorizableThing))]
    public class CategorizableThingRuleChecker : RuleChecker
    {
        /// <summary>
        /// Checks whether the <see cref="ICategorizableThing"/> is not a member of the same category more
        /// than once, including through sub-classing of categories 
        /// </summary>
        /// <param name="thing">
        /// The subject <see cref="ICategorizableThing"/>
        /// </param>
        /// <returns>
        /// An instance of <see cref="RuleCheckResult"/>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// thrown when <paramref name="thing"/> is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// thrown when <paramref name="thing"/> is not an <see cref="ICategorizableThing"/>
        /// </exception>
        [Rule("MA-0300")]
        public RuleCheckResult CheckWhetherThereAreNoDuplicateCategoriesAreDefined(Thing thing)
        {
            if (thing == null)
            {
                throw new ArgumentNullException($"The {nameof(thing)} may not be null");
            }

            var categorizableThing = thing as ICategorizableThing;
            if (categorizableThing == null)
            {
                throw new ArgumentException($"{nameof(thing)} with Iid:{thing.Iid} is not an ICategorizableThing");
            }

            var ruleAttribute = System.Reflection.MethodBase.GetCurrentMethod().GetCustomAttribute<RuleAttribute>();
            var rule = StaticRuleProvider.QueryRules().Single(r => r.Id == ruleAttribute.Id);

            //find if there are any duplicates:
            List<Category> duplicates = categorizableThing.Category.GroupBy(s => s).SelectMany(grp => grp.Skip(1)).Distinct().ToList();
            if (duplicates.Any())
            {
                var duplicateIdentifiers = string.Join(",", duplicates.Select(r => r.Iid));
                var duplicateShortNames = string.Join(",", duplicates.Select(r => r.ShortName));

                return  new RuleCheckResult(thing, rule.Id, $"The CategorizableThing is a member of the following Categories: {duplicateIdentifiers}; with shortNames: {duplicateShortNames} more than once", SeverityKind.Warning);
            }

            // verify whether a CategorizableThing is a member of a category and its supercategory by means of the Category property.
            duplicates = new List<Category>();
            foreach (var category in categorizableThing.Category.ToList())
            {
                foreach (var superCategory in category.AllSuperCategories())
                {
                    if (categorizableThing.Category.Any(x => x.Iid == superCategory.Iid))
                    {
                        duplicates.Add(category);
                        duplicates.Add(superCategory);
                    }
                }
            }
            duplicates = duplicates.Distinct().ToList();
            if (duplicates.Any())
            {
                var duplicateIdentifiers = string.Join(",", duplicates.Select(r => r.Iid));
                var duplicateShortNames = string.Join(",", duplicates.Select(r => r.ShortName));

                return new RuleCheckResult(thing, rule.Id, $"The CategorizableThing is a member of the following Categories: {duplicateIdentifiers}; with shortNames: {duplicateShortNames} more than once", SeverityKind.Warning);
            }

            return null;
        }

        /// <summary>
        /// Checks whether the <see cref="ICategorizableThing"/> is not a member of an abstract category
        /// </summary>
        /// <param name="thing">
        /// The subject <see cref="ICategorizableThing"/>
        /// </param>
        /// <returns>
        /// An instance of <see cref="RuleCheckResult"/>
        /// <exception cref="ArgumentNullException">
        /// thrown when <paramref name="thing"/> is null
        /// </exception>
        /// </returns>
        /// <exception cref="ArgumentException">
        /// thrown when <paramref name="thing"/> is not an <see cref="ICategorizableThing"/>
        /// </exception>
        [Rule("MA-0310")]
        public RuleCheckResult ChecksWheterACategorizableThingIsNotAMemberOfAnAbstractCategory(Thing thing)
        {
            if (thing == null)
            {
                throw new ArgumentNullException($"The {nameof(thing)} may not be null");
            }

            var categorizableThing = thing as ICategorizableThing;
            if (categorizableThing == null)
            {
                throw new ArgumentException($"{nameof(thing)} with Iid:{thing.Iid} is not an ICategorizableThing");
            }

            var ruleAttribute = System.Reflection.MethodBase.GetCurrentMethod().GetCustomAttribute<RuleAttribute>();
            var rule = StaticRuleProvider.QueryRules().Single(r => r.Id == ruleAttribute.Id);

            var abstractCategories = categorizableThing.Category.Where(c => c.IsAbstract);
            if (abstractCategories.Any())
            {
                var abstractIdentifiers = string.Join(",", abstractCategories.Select(r => r.Iid));
                var abstractShortNames = string.Join(",", abstractCategories.Select(r => r.ShortName));

                return new RuleCheckResult(thing, rule.Id, $"The CategorizableThing is a member of the following abstract Categories: {abstractIdentifiers}; with shortNames: {abstractShortNames}", SeverityKind.Error);
            }

            return null;
        }
    }
}