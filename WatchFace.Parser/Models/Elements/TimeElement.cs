﻿using System.Drawing;
using NLog;
using WatchFace.Parser.Utils;

namespace WatchFace.Parser.Models.Elements
{
    public class TimeElement : ContainerElement
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public TimeElement(Parameter parameter, Element parent = null, string name = null) :
            base(parameter, parent, name) { }

        public TwoDigitsElement Hours { get; set; }
        public TwoDigitsElement Minutes { get; set; }
        public TwoDigitsElement Seconds { get; set; }
        public AmPmElement AmPm { get; set; }
        public long? DrawingOrder { get; set; }
        public ImageElement Delimiter { get; set; }

        public CoordinatesElement PM { get; set; }

        public override void Draw(Graphics drawer, Bitmap[] images, WatchState state)
        {
            AmPm?.Draw(drawer, images, state);

            var hours = AmPm == null ? state.Time.Hour : state.Time.Hour % 12;
            var drawingOrder = DrawingOrder ?? 0x1234;

            foreach (var position in DrawingOrderIterator.Iterate(drawingOrder))
                switch (position)
                {
                    case DrawingOrderPosition.HourTens:
                        Hours?.Tens?.Draw(drawer, images, hours % 100 / 10);
                        break;
                    case DrawingOrderPosition.HourOnes:
                        Hours?.Ones?.Draw(drawer, images, hours % 10);
                        break;
                    case DrawingOrderPosition.Delimiter:
                        Delimiter?.Draw(drawer, images);
                        break;
                    case DrawingOrderPosition.MinuteTens:
                        Minutes?.Tens?.Draw(drawer, images, state.Time.Minute % 100 / 10);
                        break;
                    case DrawingOrderPosition.MinuteOnes:
                        Minutes?.Ones?.Draw(drawer, images, state.Time.Minute % 10);
                        break;
                    case DrawingOrderPosition.PM:
                        //PM?.Draw(drawer, images);
                        break;
                    default:
                        Logger.Warn("Not supported element {0} in DrawingOrder value", position);
                        break;
                }

            Seconds?.Draw(drawer, images, state.Time.Second);
        }

        protected override Element CreateChildForParameter(Parameter parameter)
        {
            switch (parameter.Id)
            {
                case 1:
                    Hours = new TwoDigitsElement(parameter, this, nameof(Hours));
                    return Hours;
                case 2:
                    Minutes = new TwoDigitsElement(parameter, this, nameof(Minutes));
                    return Minutes;
                case 3:
                    Seconds = new TwoDigitsElement(parameter, this, nameof(Seconds));
                    return Seconds;
                case 4:
                    AmPm = new AmPmElement(parameter, this, nameof(AmPm));
                    return AmPm;
                case 5:
                    DrawingOrder = parameter.Value;
                    return new ValueElement(parameter, this, nameof(DrawingOrder));
                case 10:
                    Delimiter = new ImageElement(parameter, this, nameof(Delimiter));
                    return Delimiter;
                case 12:
                    PM = new CoordinatesElement(parameter, this, nameof(PM));
                    return PM;
                default:
                    return base.CreateChildForParameter(parameter);
            }
        }
    }
}