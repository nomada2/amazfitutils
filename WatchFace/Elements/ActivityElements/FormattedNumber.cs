﻿using System;
using System.Collections.Generic;
using NLog;
using WatchFace.Elements.BasicElements;
using WatchFace.Models;

namespace WatchFace.Elements.ActivityElements
{
    public class FormattedNumber
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public Number Number { get; set; }
        public long SuffixImageIndex { get; set; }
        public long DecimalPointImageIndex { get; set; }

        public static FormattedNumber Parse(List<Parameter> descriptor, string path)
        {
            Logger.Trace("Reading {0}", path);

            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            var result = new FormattedNumber();
            foreach (var parameter in descriptor)
            {
                var currentPath = string.Concat(path, '.', parameter.Id.ToString());
                switch (parameter.Id)
                {
                    case 1:
                        result.Number = Number.Parse(parameter.Children, currentPath);
                        break;
                    case 2:
                        result.SuffixImageIndex = parameter.Value;
                        break;
                    case 3:
                        result.DecimalPointImageIndex = parameter.Value;
                        break;
                    default:
                        throw new InvalidParameterException(parameter, path);
                }
            }
            return result;
        }
    }
}