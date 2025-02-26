﻿/*
 * SonarQube Roslyn SDK
 * Copyright (C) 2015-2025 SonarSource SA
 * mailto:info AT sonarsource DOT com
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SonarQube.Plugins.PluginGeneratorTests
{
    [TestClass]
    public class PluginKeyUtilitiesTests
    {
        #region Tests

        [TestMethod]
        public void PluginKey_InputCanBeCorrected_ValidKeyReturned()
        {
            // Already valid
            TestGetValidKey("1234567890abc", "1234567890abc");
            TestGetValidKey("a1b2c3", "a1b2c3");

            // Spaces stripped
            TestGetValidKey(" aaa bbb ccc 111 ", "aaabbbccc111");

            // Non-alpha stripped
            TestGetValidKey("foo.analyzers", "fooanalyzers");
            TestGetValidKey("x!\"£$%^&*()_+", "x");

            // Case-changed
            TestGetValidKey("Bar.Analyzers", "baranalyzers");
            TestGetValidKey("XxX", "xxx");
        }

        [TestMethod]
        public void PluginKey_InputCannotBeCorrected_Throws()
        {
            CheckGetValidKeyThrows(null);
            CheckGetValidKeyThrows("");
            CheckGetValidKeyThrows("....");
            CheckGetValidKeyThrows("~@{}");
        }

        [TestMethod]
        public void PluginKey_ThrowsOnInvalid()
        {
            CheckThrowIfInvalidThrows(null);
            CheckThrowIfInvalidThrows("");

            CheckThrowIfInvalidThrows("Foo.Analyzers");
            CheckThrowIfInvalidThrows(" aaa bbb ccc 111 ");
            CheckThrowIfInvalidThrows("x!\"£$%^&*()_+");
        }

        #endregion Tests

        #region Private methods

        private static void TestGetValidKey(string input, string expected)
        {
            string actual = PluginKeyUtilities.GetValidKey(input);
            actual.Should().Be(expected, "Unexpected plugin key returned");

            PluginKeyUtilities.ThrowIfInvalid(expected); // should not throw on values returned by GetValidKey
        }

        private static void CheckGetValidKeyThrows(string input)
        {
            // Should throw on input that cannot be corrected
            Action action = () => PluginKeyUtilities.GetValidKey(input);
            action.Should().Throw<ArgumentException>();
        }

        private static void CheckThrowIfInvalidThrows(string input)
        {
            Action action = () => PluginKeyUtilities.ThrowIfInvalid(input);
            action.Should().ThrowExactly<ArgumentException>();
        }

        #endregion Private methods
    }
}