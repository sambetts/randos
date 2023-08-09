
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Models
{
    public class ScoringRule
    {
        public string Description { get; set; }

        public Condition Condition { get; set; }
        public Reaction Reaction { get; set; }

        public override string ToString()
        {
            return $"When {Condition}; {Reaction}";
        }

        /// <summary>
        /// Basic bitch validation. Just checks there's a known operator.
        /// </summary>
        public bool IsValidRule
        {
            get
            {
                return this.Condition.Logic != Condition.LogicalOperation.Unknown;
            }
        }

        /// <summary>
        /// Does the value hit the supplied rule or not?
        /// </summary>
        internal bool IsCompliant(AnswerDataOnlyTreeNode answerDTO)
        {
            string[] props = this.Condition.Property.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            return ConditionMatchesPropertyTree(answerDTO, props.ToList(), this.Condition);
        }


        private bool ConditionMatchesPropertyTree(object parentVal, List<string> remainingProps, Condition condition)
        {
            //if (parentVal == null)
            //{
            //    return false;
            //}

            string propName = remainingProps[0];
            PropertyInfo p = parentVal.GetType().GetProperty(propName);

            if (p == null)
            {
                throw new ArgumentOutOfRangeException(propName, $"No property by name '{propName}' exists on type '{parentVal.GetType().Name}'");
            }
            var propVal = p.GetValue(parentVal);

            // Are the sub-properties to look at?
            if (remainingProps.Count > 1)
            {
                // Only parse down if this property has a value.
                if (propVal != null)
                {
                    return ConditionMatchesPropertyTree(propVal, remainingProps.Skip(1).ToList(), condition);
                }
                else
                {
                    // We haven't found the final property, but we won't because one of the parent properties is null
                    return false;
                }
            }
            else
            {
                // At root value. Test against condition
                double doubleVal = 0d;

                bool valIsNumeric = false;

                // Check for nulls
                if (propVal != null)
                {
                    valIsNumeric = double.TryParse(propVal.ToString(), out doubleVal);
                }


                if (valIsNumeric)
                {
                    double conditionVal = double.Parse(condition.Value.ToString());
                    switch (condition.Logic)
                    {
                        case Condition.LogicalOperation.GreaterThan:
                            return (doubleVal > conditionVal);
                        case Condition.LogicalOperation.LessThan:
                            return (doubleVal < conditionVal);
                        case Condition.LogicalOperation.Equals:
                            return (doubleVal == conditionVal);
                        case Condition.LogicalOperation.NotEquals:
                            return (doubleVal != conditionVal);
                        default:
                            throw new NotSupportedException("No idea what's going on");
                    }
                }
                else
                {
                    switch (condition.Logic)
                    {
                        case Condition.LogicalOperation.GreaterThan:
                            throw new InvalidOperationException("Not a valid comparison for non-numeric values");
                        case Condition.LogicalOperation.LessThan:
                            throw new InvalidOperationException("Not a valid comparison for non-numeric values");
                        case Condition.LogicalOperation.Equals:
                            return (propVal == condition.Value);
                        case Condition.LogicalOperation.NotEquals:
                            return (propVal != condition.Value);
                        default:
                            throw new NotSupportedException("No idea what's going on");
                    }
                }
            }
        }
    }

    public class ScoringRules : List<ScoringRule>
    {

        /// <summary>
        /// Only for Json convertion
        /// </summary>
        public ScoringRules() { }


        public static ScoringRules LoadRules()
        {
            ScoringRules rulesData = Newtonsoft.Json.JsonConvert.DeserializeObject<ScoringRules>(ScoringRuleData.JsonData);
            return rulesData;
        }
    }
}
