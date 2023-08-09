using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public static class Constants
    {
        public static string RESPONSE_DISAGREE { get { return "Disagree with proposition"; } }
        public static string RESPONSE_AGREE { get { return "Agree with proposition"; } }

        public static string RESPONSE_DISAGREE_WITH_EVIDENCE { get { return "Evidence is bullshit"; } }
        public static string RESPONSE_AGREE_WITH_EVIDENCE { get { return "Evidence is totes awesome"; } }

        public static string RESPONSE_IRRELEVANT { get { return "Irrelevant argument"; } }

        public static string SEARCH_INDEX_NAME { get { return "answers"; } }
    }
}
