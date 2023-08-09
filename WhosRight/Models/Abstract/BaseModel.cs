using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Abstract
{
    public abstract class BaseModel : IEquatable<BaseModel>
    {
        public int ID { get; set; }

        public bool Equals(BaseModel other)
        {
            if (other == null)
            {
                return false;
            }
            return other.ID.Equals(ID);
        }

        public override bool Equals(object obj)
        {
            if (obj is BaseModel)
            {
                return Equals((BaseModel)obj);
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return ID;
        }
    }
}
