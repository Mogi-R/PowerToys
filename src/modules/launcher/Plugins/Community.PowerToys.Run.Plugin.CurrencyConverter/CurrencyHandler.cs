﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnitsNet;

namespace Community.PowerToys.Run.Plugin.CurrencyConverter
{
    public static class CurrencyHandler
    {
        private static readonly int _roundingFractionalDigits = 4;

        private static readonly QuantityType[] _included = new QuantityType[]
        {
            QuantityType.Acceleration,
            QuantityType.Angle,
            QuantityType.Area,
            QuantityType.Duration,
            QuantityType.Energy,
            QuantityType.Information,
            QuantityType.Length,
            QuantityType.Mass,
            QuantityType.Power,
            QuantityType.Pressure,
            QuantityType.Speed,
            QuantityType.Temperature,
            QuantityType.Volume,
        };

        /// <summary>
        /// Given string representation of unit, converts it to the enum.
        /// </summary>
        /// <returns>Corresponding enum or null.</returns>
        private static Enum GetUnitEnum(string unit, QuantityInfo unitInfo)
        {
            UnitInfo first = Array.Find(unitInfo.UnitInfos, info => info.Name.ToLower() == unit.ToLower());
            if (first != null)
            {
                return first.Value;
            }

            if (UnitParser.Default.TryParse(unit, unitInfo.UnitType, out Enum enum_unit))
            {
                return enum_unit;
            }

            return null;
        }

        /// <summary>
        /// Given parsed ConvertModel, computes result. (E.g "1 foot in cm").
        /// </summary>
        /// <returns>The converted value as a double.</returns>
        public static double ConvertInput(ConvertModel convertModel, QuantityType quantityType)
        {
            QuantityInfo unitInfo = Quantity.GetInfo(quantityType);

            var fromUnit = GetUnitEnum(convertModel.FromUnit, unitInfo);
            var toUnit = GetUnitEnum(convertModel.ToUnit, unitInfo);

            if (fromUnit != null && toUnit != null)
            {
                return 123; // UnitsNet.CurrencyConverter.Convert(convertModel.Value, fromUnit, toUnit);
            }

            return double.NaN;
        }

        /// <summary>
        /// Given ConvertModel returns collection of possible results.
        /// </summary>
        /// <returns>The converted value as a double.</returns>
        public static IEnumerable<CurrencyConversionResult> Convert(ConvertModel convertModel)
        {
            var results = new List<CurrencyConversionResult>();
            foreach (QuantityType quantityType in _included)
            {
                double convertedValue = CurrencyHandler.ConvertInput(convertModel, quantityType);

                if (!double.IsNaN(convertedValue))
                {
                    CurrencyConversionResult result = new CurrencyConversionResult(Math.Round(convertedValue, _roundingFractionalDigits), convertModel.ToUnit, quantityType);
                    results.Add(result);
                }
            }

            return results;
        }
    }
}
