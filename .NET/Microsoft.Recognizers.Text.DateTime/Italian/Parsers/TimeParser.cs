﻿using DateObject = System.DateTime;

namespace Microsoft.Recognizers.Text.DateTime.Italian
{ 
    public class TimeParser : BaseTimeParser
    {
        public TimeParser(ITimeParserConfiguration configuration) : base(configuration) { }

        protected override DateTimeResolutionResult InternalParse(string text, DateObject referenceTime)
        {
            var innerResult = base.InternalParse(text, referenceTime);
            if (!innerResult.Success)
            {
                innerResult = ParseIsh(text, referenceTime);
            }
            return innerResult;
        }

        // parse "noonish", "11-ish"
        // fr - peu pres midi ("approximately noon")- IshRegex for 'peu pres'
        private DateTimeResolutionResult ParseIsh(string text, DateObject referenceTime)
        {
            var ret = new DateTimeResolutionResult();
            var trimmedText = text.ToLowerInvariant().Trim();

            var match = ItalianTimeExtractorConfiguration.IshRegex.Match(trimmedText);
            if (match.Success && match.Length == trimmedText.Length)
            {
                var hourStr = match.Groups["hour"].Value;
                var hour = 12;
                if (!string.IsNullOrEmpty(hourStr))
                {
                    hour = int.Parse(hourStr);
                }

                ret.Timex = "T" + hour.ToString("D2");
                ret.FutureValue =
                    ret.PastValue =
                        DateObject.MinValue.SafeCreateFromValue(referenceTime.Year, referenceTime.Month, referenceTime.Day, hour, 0, 0);
                ret.Success = true;
                return ret;
            }

            return ret;
        }
    }
}
