

namespace Models
{

    public class Condition
    {
        public string Property { get; set; }
        public string LogicalOp { get; set; }
        public LogicalOperation Logic
        {
            get
            {
                switch (this.LogicalOp.ToLower())
                {
                    case "greaterthan":
                        return LogicalOperation.GreaterThan;
                    case "lessthan":
                        return LogicalOperation.LessThan;
                    case "equals":
                        return LogicalOperation.Equals;
                    case "notequals":
                        return LogicalOperation.NotEquals;
                    default:
                        return LogicalOperation.Unknown;
                }
            }
        }
        public object Value { get; set; }

        public override string ToString()
        {
            return $"'{Property}' {Logic} '{Value}'";
        }

        public enum LogicalOperation
        {
            Unknown,
            GreaterThan,
            LessThan,
            Equals,
            NotEquals
        }
    }

    public class Reaction
    {
        public int PointsModifier { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            if (PointsModifier > 0)
            {
                return $"(+{PointsModifier}): {Message}";
            }
            else if (PointsModifier == 0)
            {
                return $"(no points): {Message}";
            }
            else
            {
                return $"(-{PointsModifier}): {Message}";
            }
        }
    }

}

